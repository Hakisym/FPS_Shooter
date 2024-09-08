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
    [HideInInspector] public bool reloading, aiming ,chambering;
    public bool canShoot;
    [HideInInspector] public bool shootState { get => !readyToShoot; }
    [SerializeField] bool shooting;
    [SerializeField] bool readyToShoot;
    private bool wasShooting;
    public bool IsShooting;

    [Header("Reference")]
    private Transform muzzlePoint;
    private Transform castingPoint;
    private Transform prefabHolder;
    [Space(5)]
    public Camera playerCam;
    public LayerMask whatIsShootable;
    public Animator animator;
    [Space(5)]
    private Actor actor;
    private Inventory inventory;
    private PlayerMovement pm;
    private WeaponRecoil weaponRecoil;
    private CameraManager cameraManager;
    private CameraRecoil cameraRecoil;

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

    //gun spread
    [HideInInspector] public float spread;
    [HideInInspector] public float currentSpread;
    private float bulletSpread;
    [SerializeField] private float resizeSpeed = 3f;

    public Action OnStartShoot;
    public Action OnEndShoot;
    public UnityEvent OnFire;
    public Action OnAimStart;
    public Action OnAimFinish;
    public Action OnAimCancel;

    private void Awake()
    {
        inventory = GetComponentInChildren<Inventory>();
        actor = GetComponent<Actor>();
        pm = GetComponent<PlayerMovement>();
        cameraManager = GetComponentInChildren<CameraManager>();
        cameraRecoil = cameraManager.cameraRecoil;

        fireAudio.Equip(gameObject, null);
        fireTailAudio.Equip(gameObject, null);
        reloadAudio.Equip(gameObject, null);
    }

    private void Start()
    {
        readyToShoot = true;
        canShoot = true;
        prefabHolder = FindObjectOfType<PoolManager>().poolHolder;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.canShoot = !paused;

        //disable aim on sprint
        pm.OnSprint.AddListener(() => aiming = false);
        GetComponent<HealthSystem>().OnDead.AddListener(() => inventory.DropGun());
    }

    private void Update()
    {
        MyInput();
        UpdateSpread();
        GunState();
    }

    private void GunState()
    {
        if (!readyToShoot)
        {
            if (!wasShooting) OnStartShoot?.Invoke();
            wasShooting = true;
            IsShooting = true;
        }
        else
        {
            if (wasShooting) OnEndShoot?.Invoke();
            wasShooting = false;
            IsShooting = false;
        }
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

        if (reloading || inventory.switching || bulletsLeft <= 0 || chambering)
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
        if (preset.gunStats.fireMode != ShootMode.PumpAction)
        {
            Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);
            inventory.currentWeapon.animator.Play("Fire");
        }

        //spread
        bulletSpread += preset.gunStats.bulletSpreadOneShot;

        //cam shake
        cameraManager.ShakeCameras(preset.cameraShake,preset.cameraFadeoutTime);

        //Recoil
        cameraRecoil.ApplyRecoil();
        weaponRecoil.Fire();

        //play shoot sound
        fireAudio.Update(currentFireAudio);
        fireAudio.PlayOneShot(currentFireAudio);

        fireTailAudio.Stop();
        fireTailAudio.PlayOneShot(preset.soundClips.fireTail);

        bulletsLeft--;
        bulletsShot--;

        if (preset.gunStats.fireMode == ShootMode.PumpAction)
            PumpAction();

        Invoke("ResetShoot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", preset.gunStats.timeBetweenShots);
    }

    private void PumpAction()
    {
        chambering = true;
        StartCoroutine(Pump());
        
        Invoke("PumpFinished", preset.gunStats.chamberTime);
    }

    IEnumerator Pump()
    {
        yield return new WaitForSeconds(preset.cameraRecoilParameters.returnDuration);
        Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);
        inventory.currentWeapon.animator.Play("Fire");
    }

    private void PumpFinished()
    {
        canShoot = true;
        chambering = false;
    }

    private void FireOneShot()
    {
        Vector3 _spread = HenryUtilities.GetSpreadDirection(spread / 2, playerCam);

        Vector3 dir = (playerCam.transform.forward + _spread).normalized;

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

                    damageable.TakeDamage(damageResult, actor);

                    if (actor.isLocallyMine)
                        UIManager.instance.OnEnemyHit.Invoke(isHeadShot, hit.point);
                }

                //verify kill
                if (!damageable.deadConfirmed && damageable.IsDead())
                {
                    if (actor)
                        actor?.onKill?.Invoke(damageable.GetActor());

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
            //bulletManager.CreateBullet(muzzlePoint.position, bulletDir * preset.gunStats.velocity, preset.gunStats.bulletDrop);
        }
        else
        {
            //get ray point from distance
            bulletDir = (ray.GetPoint(50) - muzzlePoint.position).normalized;

            //check obstacle between muzzle and hit point
            bool hasHit = Physics.Raycast(muzzlePoint.position, bulletDir, whatIsShootable);

            //render the bullet from the muzzle
            //bulletManager.CreateBullet(muzzlePoint.position, bulletDir * preset.gunStats.velocity, preset.gunStats.bulletDrop);
        }

        pelletsShot--;

        if (pelletsShot > 0)
            FireOneShot();
    }

    private void ResetShoot()
    {
        readyToShoot = true;
    }

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
