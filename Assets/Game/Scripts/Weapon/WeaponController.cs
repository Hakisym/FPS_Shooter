using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    [Header("WeaponData")]
    [HideInInspector] public WeaponData preset;

    [Header("Gun states")]
    [SerializeField] bool holdAim;
    [HideInInspector] public bool reloading, aiming ,chambering;
    [HideInInspector] public bool canShoot;
    [HideInInspector] public bool shootState { get => !readyToShoot; }
    private bool shooting;
    private bool readyToShoot;
    private bool wasShooting;

    [Header("Reference")]
    private Transform muzzlePoint;
    private Transform castingPoint;
    private Transform prefabHolder;
    [Space(5)]
    [SerializeField] Camera playerCam;
    [SerializeField] LayerMask whatIsShootable;
    [SerializeField] Animator animator;
    [Space(5)]
    private Actor actor;
    private Inventory inventory;
    private PlayerMovement pm;
    private WeaponRecoil weaponRecoil;
    private CameraManager cameraManager;
    private CameraRecoil cameraRecoil;
    private PlayerHUD hud;

    [Header("Visuals")]
    [SerializeField] GameObject metalImpact;
    [SerializeField] GameObject bloodImpact;
    [SerializeField] GameObject bulletTrail;
    private GameObject bulletCasting;
    private ParticleSystem muzzleVFX;

    [Header("Sounds")]
    private Audio fireAudio = new Audio();
    private Audio fireTailAudio = new Audio();
    private Audio weaponAudio = new Audio();

    private int bulletsLeft;
    private int bulletsShot;
    private int pelletsShot;
    private float timeBetweenShooting;

    //gun spread
    private float spread;
    private float currentSpread;
    private float bulletSpread;
    [SerializeField] float resizeSpeed = 3f;

    //events
    public Action OnStartShoot;
    public Action OnEndShoot;
    public UnityEvent OnShoot;
    public Action<WeaponData> OnWeaponChange;
    public Action<float,Vector3,Vector3,float> OnRaycastShoot;

    //gun aim
    private bool wasAiming;
    private float timeElapsed;
    public float aimValue { get; private set; }

    //network
    [SerializeField] bool isOffline = true;

    private void Awake()
    {
        inventory = GetComponentInChildren<Inventory>();
        actor = GetComponent<Actor>();
        pm = GetComponent<PlayerMovement>();
        cameraManager = GetComponentInChildren<CameraManager>();
        cameraRecoil = cameraManager.cameraRecoil;
        hud = GetComponentInChildren<PlayerHUD>();

        fireAudio.Equip(gameObject, null);
        fireTailAudio.Equip(gameObject, null);
        weaponAudio.Equip(gameObject, null);
    }

    private void Start()
    {
        readyToShoot = true;
        canShoot = true;
        prefabHolder = FindObjectOfType<PoolManager>().poolHolder;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.canShoot = !paused;

        GetComponent<HealthSystem>()?.OnDead.AddListener(() => inventory.DropGun());
    }

    private void Update()
    {
        MyInput();
        UpdateSpread();
        GunState();
        AimState();
        DisplayHud();
        DisplayCrosshair();
    }

    private void AimState()
    {
        if (!preset) return;

        //Update aim percentage
        if (aiming)
        {
            timeElapsed += Time.deltaTime;

            if (!wasAiming)
                weaponAudio.PlayOneShot(preset.soundClips.aimSound);
            wasAiming = true;
        }
        else
        {
            timeElapsed -= Time.deltaTime;

            wasAiming = false;
        }

        float aimDuration = preset.gunStats.adsTime;

        //clamp ads time
        timeElapsed = Mathf.Clamp(timeElapsed, 0, aimDuration);
        //ads gradually over time
        aimValue = timeElapsed / aimDuration;

        cameraManager.cameraFovManager.UpdateAimFov(aimValue);
        UpdateAimSpread(aimValue);
    }

    private void GunState()
    {
        if (!readyToShoot)
        {
            if (!wasShooting) OnStartShoot?.Invoke();
            wasShooting = true;
        }
        else
        {
            if (wasShooting) OnEndShoot?.Invoke();
            wasShooting = false;
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

        bulletsLeft = preset.gunStats.magazineSize;
        timeBetweenShooting = 60f / preset.gunStats.fireRate;

        OnWeaponChange?.Invoke(wid.preset);
    }

    private void MyInput()
    {
        if (!preset) return;

        if(preset.gunStats.fireMode == ShootMode.Auto) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (holdAim)
        {
            aiming = Input.GetMouseButton(1);
        }
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

        OnShoot?.Invoke();

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
        cameraRecoil.ApplyRecoil(aiming);
        weaponRecoil.Fire();

        //play shoot sound
        fireAudio.PlayOneShot(preset.soundClips.shootSound);

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
        Ray ray = new Ray(playerCam.transform.position, dir);

        if (isOffline)
        {
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
                            UIManager.instance.hitMarker.Show(isHeadShot, hit.point);
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
            }

            //Vector3 bulletDir = hit.collider ? (hit.point - muzzlePoint.position).normalized : (ray.GetPoint(50) - muzzlePoint.position).normalized;
            //bulletManager.CreateBullet(muzzlePoint.position, bulletDir * preset.gunStats.velocity, preset.gunStats.bulletDrop);
        }
        else
        {
            OnRaycastShoot?.Invoke(preset.gunStats.damage, playerCam.transform.position, dir, preset.gunStats.range);
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
        weaponAudio.PlayOneShot(preset.soundClips.reloadSoundAmmoLeft);

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
        if (pm.moveState == MoveState.InAir) locomotionSpread = preset.gunStats.jumpSpread;
        else if(pm.moveState == MoveState.Crouching) locomotionSpread = preset.gunStats.crouchSpread;
        else if (pm.moveState == MoveState.Idle) locomotionSpread = preset.gunStats.defaultSpread;
        else if (pm.moveState == MoveState.Walking) locomotionSpread = preset.gunStats.walkSpread;
        else if (pm.moveState == MoveState.Sprinting) locomotionSpread = preset.gunStats.runSpread;

        //if (weaponController.aiming) locomotionSpread *= weaponController.preset.gunStats.aimSpreadMultiplier;

        bulletSpread = Mathf.Lerp(bulletSpread, 0f, preset.gunStats.bulletSpreadResizeSpeed * Time.deltaTime);

        currentSpread = Mathf.Lerp(currentSpread, locomotionSpread + bulletSpread, resizeSpeed * Time.deltaTime);
    }

    private void UpdateAimSpread(float t)
    {
        spread = Mathf.Lerp(currentSpread / 1000, currentSpread / 1000 * preset.gunStats.aimSpreadMultiplier, t);
    }

    private void DisplayCrosshair()
    {
        hud.crosshair.DisplayCrosshair(currentSpread, !aiming && preset);
    }

    private void DisplayHud()
    {
        hud.weaponHUD.SetAmmoValue(bulletsLeft, preset);
    }

    public float GetAimPercentage()
    {
        return aimValue;
    }

    public void CanShoot() => canShoot = true;
}
