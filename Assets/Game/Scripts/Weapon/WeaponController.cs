using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    [Header("WeaponData")]
    [HideInInspector] public WeaponData preset;

    [Header("Gun states")]
    [SerializeField] bool holdAim;
    [SerializeField] bool crosshairFollowRecoil;
    [HideInInspector] public bool reloading, aiming;
    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool shootState { get => !readyToShoot; }
    [HideInInspector] public bool shooting;
    private bool readyToShoot;

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
    public GameObject bulletTrail;
    private GameObject bulletCasting;
    private ParticleSystem muzzleVFX;

    [Header("Sounds")]
    public Audio fireAudio = new Audio(); 
    public Audio fireTailAudio = new Audio(); 
    public Audio reloadAudio = new Audio(); 
    private AudioProfile currentFireAudio;

    [HideInInspector] public int bulletsLeft;
    private int bulletsShot;
    private int pelletsShot;
    private float timeBetweenShooting;
    private float maxLifetime = 3.0f;
    private List<Bullet> bullets = new List<Bullet>();

    //gun spread
    [HideInInspector] public float spread;
    [HideInInspector] public float currentSpread;
    private float bulletSpread;
    [SerializeField] private float resizeSpeed = 3f;

    public Action OnFire;

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        weaponHUD = GetComponentInChildren<PlayerHUD>();

        fireAudio.Equip(gameObject, null);
        fireTailAudio.Equip(gameObject, null);
        reloadAudio.Equip(gameObject, null);
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
        UpdateSpread();
    }

    private void FixedUpdate()
    {
        UpdateBullets(Time.fixedDeltaTime);
    }

    public void GetWeaponData(WeaponIdentification wid)
    {
        preset = wid.preset;
        muzzlePoint = wid.muzzle;
        castingPoint = wid.castPoint;

        //get muzzle flash
        muzzleVFX = muzzlePoint.GetComponentInChildren<ParticleSystem>();
        bulletCasting = preset.bulletCasting;

        weaponRecoil = wid.GetComponentInChildren<WeaponRecoil>();
        currentFireAudio = preset.soundClips.shootSound;

        bulletsLeft = preset.gunStats.magazineSize;
        timeBetweenShooting = 60f / preset.gunStats.fireRate;
    }

    private void MyInput()
    {
        if (!preset) return;

        if(preset.gunStats.fireMode == ShootMode.Auto) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (holdAim) aiming = Input.GetMouseButton(1) && !pm.isSprinting;
        else if (Input.GetMouseButtonDown(1)) aiming = !aiming;

        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < preset.gunStats.magazineSize && !inventory.switching) Reload();

        if (readyToShoot && shooting && canShoot)
        {
            bulletsShot = preset.gunStats.bulletsPerTap;
            pelletsShot = preset.gunStats.pelletsPerBullet;
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

        OnFire?.Invoke();

        FireOneShot();

        //muzzle flash
        muzzleVFX.Play();

        //bullet cast and gun animation
        if (preset.gunStats.fireMode != ShootMode.BoltAction)
        {
            Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);
            inventory.currentWeapon.animator.Play("Fire");
        }

        //spread
        bulletSpread += preset.gunStats.bulletSpreadOneShot;

        //cam shake
        characterManager.cameraManager.ShakeCameras(preset.cameraShake,preset.cameraFadeoutTime);

        //Recoil
        cameraRecoil.ApplyRecoil(preset.cameraRecoilParameters.verticalRecoil, preset.cameraRecoilParameters.hotizontalRecoil, preset.cameraRecoil, aiming);
        weaponRecoil.Fire();

        //play shoot sound
        fireAudio.Update(currentFireAudio);
        fireAudio.PlayOneShot(currentFireAudio);

        fireTailAudio.Stop();
        fireTailAudio.PlayOneShot(preset.soundClips.fireTail);

        bulletsLeft--;
        bulletsShot--;

        if (preset.gunStats.fireMode == ShootMode.BoltAction)
            Invoke("BoldAction", preset.gunStats.chamberTime);

        Invoke("ResetShoot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", preset.gunStats.timeBetweenShots);
    }

    private void BoldAction()
    {
        Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);
        inventory.currentWeapon.animator.Play("Fire");
    }

    private void FireOneShot()
    {
        Vector3 _spread = HenryUtilities.GetSpreadDirection(spread / 2, playerCam);

        Vector3 dir;
        if (!crosshairFollowRecoil) dir = (playerCam.transform.forward + _spread).normalized;
        else dir = aiming ? (playerCam.transform.forward + _spread).normalized : (recoilPoint.transform.forward + _spread).normalized;

        RaycastHit hit;
        Vector3 bulletDir;
        Ray ray = new Ray(playerCam.transform.position, dir);


        // Hitscan shoot
        if (Physics.Raycast(ray, out hit, preset.gunStats.range, whatIsShootable))
        {
            //damageable group
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
            {
                //decal
                Instantiate(bloodImpact, hit.point, Quaternion.LookRotation(hit.normal), prefabHolder);

                //check damageable
                IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();
                bool isAlive = damageable != null ? !damageable.IsDead() : false;

                //deal damage
                if (isAlive)
                {
                    bool isHeadShot = hit.collider.CompareTag("Head");
                    float damageMultiplier = isHeadShot ? 1.5f : 1f;
                    float damageResult = preset.gunStats.damage * damageMultiplier;

                    damageable.TakeDamage(damageResult, Actor);

                    if (Actor.isLocallyMine)
                        UIManager.instance.OnEnemyHit.Invoke(isHeadShot, hit.point);
                }

                //verify kill
                if (!damageable.deadConfirmed && damageable.IsDead())
                {
                    if (Actor)
                        Actor?.onKill?.Invoke(damageable.GetActor());

                    damageable.deadConfirmed = true;
                }

                if (hit.rigidbody)
                {
                    hit.rigidbody.AddForceAtPosition(-hit.normal * preset.gunStats.impactForce, hit.point, ForceMode.Impulse);
                }
            }

            //bullet impact
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
                PoolManager.instance.SpawnGameObject(PoolTag.BulletImpact, hit.point, Quaternion.LookRotation(hit.normal));

            //bullet tracer
            bulletDir = (hit.point - muzzlePoint.position).normalized;
            var bullet = new Bullet(muzzlePoint.position, bulletDir * preset.gunStats.velocity, bulletTrail, prefabHolder);
            bullets.Add(bullet);
        }
        else
        {
            //get ray point from distance
            bulletDir = (ray.GetPoint(50) - muzzlePoint.position).normalized;

            //check obstacle between muzzle and hit point
            bool hasHit = Physics.Raycast(muzzlePoint.position, bulletDir, whatIsShootable);

            //render the bullet from the muzzle
            var bullet = new Bullet(muzzlePoint.position, bulletDir * preset.gunStats.velocity, bulletTrail, prefabHolder);
            bullets.Add(bullet);
        }

        pelletsShot--;

        if (pelletsShot > 0)
            FireOneShot();
    }

    private void ResetShoot() => readyToShoot = true;

    private void Reload()
    {
        reloading = true;
        animator.SetTrigger("reloading");

        //play reload sound
        reloadAudio.PlayOneShot(preset.soundClips.reloadSoundAmmoLeft);

        Invoke("ReloadFinished", preset.gunStats.reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = preset.gunStats.magazineSize;

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
            Vector3 p0 = bullet.GetPosition(preset.gunStats.bulletDrop);
            bullet.time += deltaTime;
            Vector3 p1 = bullet.GetPosition(preset.gunStats.bulletDrop);
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

    private void UpdateSpread()
    {
        if (!preset) return;

        float locomotionSpread = 0;

        // Manage different sizes
        if (pm.isGrounded)
        {
            if (pm.currentSpeed == pm.crouchSpeed) locomotionSpread = preset.gunStats.crouchSpread;
            else if (pm.moveVel < 0.2f) locomotionSpread = preset.gunStats.defaultSpread;
            else if (pm.currentSpeed == pm.walkSpeed) locomotionSpread = preset.gunStats.walkSpread;
            else if (pm.currentSpeed == pm.sprintSpeed) locomotionSpread = preset.gunStats.runSpread;
        }
        else locomotionSpread = preset.gunStats.jumpSpread;

        //if (weaponController.aiming) locomotionSpread *= weaponController.preset.gunStats.aimSpreadMultiplier;

        bulletSpread = Mathf.Lerp(bulletSpread, 0f, preset.gunStats.bulletSpreadResizeSpeed * Time.deltaTime);

        currentSpread = Mathf.Lerp(currentSpread, locomotionSpread + bulletSpread, resizeSpeed * Time.deltaTime);
    }

    public void UpdateAimSpread(float t)
    {
        spread = Mathf.Lerp(currentSpread / 1000, currentSpread / 1000 * preset.gunStats.aimSpreadMultiplier, t);
    }

    public void CanShoot() => canShoot = true;
}
