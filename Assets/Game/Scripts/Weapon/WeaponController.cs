using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    [Header("WeaponData")]
    [HideInInspector] public WeaponData wd;

    [Header("Gun states")]
    [SerializeField] bool holdAim;
    [HideInInspector] public bool reloading, aiming;
    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool shootState { get => !readyToShoot; }
    private bool readyToShoot;
    private bool shooting;

    [Header("Reference")]
    private Transform muzzlePoint;
    private Transform castingPoint;
    [HideInInspector] public Transform prefabHolder;
    [Space(5)]
    public Camera playerCam;
    public LayerMask whatIsShootable;
    public Animator animator;
    [Space(5)]
    public CameraRecoil cameraRecoil;
    public RecoilPoint recoilPoint;
    public Actor Actor { get; set; }
    private CharacterManager characterManager;
    private Inventory inventory;
    private PlayerMovement pm;
    private HealthSystem healthSystem;
    private WeaponRecoil weaponRecoil;
    private PlayerHUD weaponHUD;

    [Header("Visuals")]
    public GameObject metalImpact;
    public GameObject bloodImpact;
    public TrailRenderer bulletTrail;
    public TrailRenderer bulletTrailFar;
    public GameObject bulletCasting;
    private ParticleSystem muzzleVFX;

    [Header("Sounds")]
    public AudioSource mainAudioSource;
    public AudioSource shootAudioSource;
    private AudioClip shootSound;

    [HideInInspector] public int bulletsLeft;
    private int bulletsShot;
    private float timeBetweenShooting;
    private float maxLifetime = 3.0f;
    private List<Bullet> bullets = new List<Bullet>();

    public Action OnFire;

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        weaponHUD = GetComponentInChildren<PlayerHUD>();
    }

    private void Start()
    {
        pm = characterManager.playerMovement;
        Actor = characterManager.actor;
        inventory = characterManager.inventory;
        healthSystem = characterManager.damageable as HealthSystem;

        readyToShoot = true;
        canShoot = true;
        prefabHolder = FindObjectOfType<PoolManager>().poolHolder;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.readyToShoot = !paused;

        //disable aim on sprint
        pm.OnSprint.AddListener(() => aiming = false);
        healthSystem.OnDead.AddListener(() => inventory.DropGun());
    }

    private void Update()
    {
        MyInput();
    }

    private void FixedUpdate()
    {
        UpdateBullets(Time.fixedDeltaTime);
    }

    public void GetWeaponData(WeaponIdentification wid)
    {
        wd = wid.wd;
        muzzlePoint = wid.muzzle;
        castingPoint = wid.castPoint;

        //attach muzzle flash
        muzzleVFX = muzzlePoint.GetComponentInChildren<ParticleSystem>();

        weaponRecoil = wid.GetComponentInChildren<WeaponRecoil>();

        shootSound = wd.soundClips.shootSound;
        bulletsLeft = wd.gunStats.magazineSize;
        timeBetweenShooting = 60f / wd.gunStats.fireRate;
    }

    private void MyInput()
    {
        if (!wd) return;

        if(wd.gunStats.allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (holdAim) aiming = Input.GetMouseButton(1) && !pm.isSprinting;
        else if (Input.GetMouseButtonDown(1)) aiming = !aiming;

        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < wd.gunStats.magazineSize && !inventory.switching) Reload();

        if (readyToShoot && shooting && canShoot)
        {
            bulletsShot = wd.gunStats.bulletsPerTap;
            Shoot();
        }

        if (reloading || bulletsLeft <= 0 || inventory.switching)
            canShoot = false;

        if (Input.GetKeyDown(KeyCode.Backspace) && !reloading)
            inventory.DropGun();
    }

    private void Shoot()
    {
        readyToShoot = false;
        CancelInvoke("ResetShootMode");

        OnFire?.Invoke();

        //Spread
        float bulletSpread;
        if (aiming)
            bulletSpread = 0;
        else
        {
            //stand spread
            if (weaponHUD.crosshair.locomotionSpread == weaponHUD.crosshair.defaultSpread)
                bulletSpread = (weaponHUD.crosshair.bulletSpread + wd.gunStats.bulletAccuracy) / 1000;
            //crouch spread
            else if (weaponHUD.crosshair.locomotionSpread == weaponHUD.crosshair.crouchSpread)
                bulletSpread = (weaponHUD.crosshair.bulletSpread + wd.gunStats.bulletAccuracy) / 1200;
            else
                bulletSpread = weaponHUD.crosshair.spread / 1000;
        }

        var spread = HenryUtilities.GetSpreadDirection(bulletSpread / 2, playerCam);

        Vector3 dir = aiming ? (playerCam.transform.forward + spread).normalized : (recoilPoint.transform.forward + spread).normalized;

        RaycastHit hit;
        Vector3 bulletDir;
        Ray ray = new Ray(playerCam.transform.position, dir);


        // Hitscan shoot
        if (Physics.Raycast(ray, out hit, wd.gunStats.bulletRange, whatIsShootable))
        {
            //damageable group
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
            {
                //decal
                Instantiate(bloodImpact, hit.point, Quaternion.LookRotation(hit.normal),prefabHolder);

                //check damageable
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                bool isAlive = damageable != null ? !damageable.IsDead() : false;

                //deal damage
                if (isAlive)
                {
                    float damageMultiplier = hit.collider.CompareTag("Head") ? 1.5f : 1f;
                    float damageResult = wd.gunStats.damage * damageMultiplier;

                    damageable.TakeDamage(damageResult, Actor);

                    if (Actor.isLocallyMine)
                        UIManager.instance.OnEnemyHit.Invoke(false, true);
                }

                //verify kill
                if (!damageable.deadConfirmed && damageable.IsDead())
                {
                    if(Actor)
                        Actor?.onKill?.Invoke(damageable.GetActor());

                    damageable.deadConfirmed = true;
                }

                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(-hit.normal * wd.gunStats.bulletImpactForce, hit.point, ForceMode.Impulse);
                }
            }

            //bullet impact
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
                PoolManager.instance.SpawnGameObject(PoolTag.BulletImpact, hit.point, Quaternion.LookRotation(hit.normal));

            //bullet tracer
            bulletDir = (hit.point - muzzlePoint.position).normalized;
            var bullet = new Bullet(muzzlePoint.position, bulletDir * wd.gunStats.bulletSpeed, bulletTrail, prefabHolder);
            bullets.Add(bullet);
        }
        else
        {
            //get ray point from distance
            bulletDir = (ray.GetPoint(50) - muzzlePoint.position).normalized;

            //check obstacle between muzzle and hit point
            bool hasHit = Physics.Raycast(muzzlePoint.position, bulletDir, whatIsShootable);

            //render the bullet from the muzzle
            var bullet = new Bullet(muzzlePoint.position, bulletDir * wd.gunStats.bulletSpeed, hasHit ? bulletTrail: bulletTrailFar, prefabHolder);
            bullets.Add(bullet);
        }

        //muzzle flash
        muzzleVFX.Play();

        //bullet cast
        Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);

        //crosshair
        weaponHUD.crosshair.Fire();

        //cam shake
        CamShake.instance.ShootShake(wd.camShakeIntensity);

        //Recoil
        cameraRecoil.Fire();
        weaponRecoil.Fire();

        //play shoot sound
        shootAudioSource.clip = shootSound;
        shootAudioSource.Play();

        //gun animation
        inventory.currentWeapon.animator.Play("Fire");

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShoot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", wd.gunStats.timeBetweenShots);
    }

    private void ResetShoot() => readyToShoot = true;

    private void Reload()
    {
        reloading = true;
        animator.SetTrigger("reloading");

        //play reload sound
        mainAudioSource.clip = wd.soundClips.reloadSoundAmmoLeft;
        mainAudioSource.Play();

        Invoke("ReloadFinished", wd.gunStats.reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = wd.gunStats.magazineSize;

        canShoot =true;
        reloading = false;
    }

    private void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet =>
        {
            if (bullet.time >= maxLifetime)
            {
                //Call event on destroy
                bullet.OnDestroy();
                return true;
            }
            return false;
        });
    }

    private void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = bullet.GetPosition(wd.gunStats.bulletDrop);
            bullet.time += deltaTime;
            Vector3 p1 = bullet.GetPosition(wd.gunStats.bulletDrop);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void RaycastSegment(Vector3 start,Vector3 end,Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        //Raycast
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, distance, whatIsShootable))
        {
            bullet.UpdateTracer(hit.point);
            bullet.time = maxLifetime;
        }
        else
        {
            bullet.UpdateTracer(end);
        }
    }

    public void CanShoot() => canShoot = true;
}
