using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour
{
    [Header("WeaponData")]
    public WeaponData wd;

    [Header("Gun states")]
    [HideInInspector]public bool shooting, reloading, aiming;
    public bool readyToShoot;
    public bool canShoot;

    [Header("Reference")]
    public Camera playerCam;
    public Transform prefabHolder;
    public Transform muzzlePoint;
    public Transform castingPoint;
    public ParticleSystem muzzleFlash;
    public LayerMask whatIsShootable;
    public Animator animator;
    public CameraRecoil cameraRecoil;
    public WeaponRecoil weaponRecoil;
    public Inventory inventory;

    [Header("Visuals")]
    public ParticleSystem muzzleFlashPrefab;
    public GameObject bulletImpact;
    public TrailRenderer bulletTrail;
    public GameObject bulletCasting;

    [Header("Sounds")]
    public AudioSource mainAudioSource;
    public AudioSource shootAudioSource;
    private AudioClip shootSound;

    public int bulletsLeft;
    private int bulletsShot;
    private float timeBetweenShooting;
    private float maxLifetime = 3.0f;
    private List<Bullet> bullets = new List<Bullet>();

    public Action<WeaponIdentification> OnWeaponEquip;

    private void Awake()
    {
        readyToShoot = true;
        canShoot = true;
        muzzleFlash = Instantiate(muzzleFlashPrefab, this.transform);
    }

    private void Update()
    {
        MyInput();

        //debug for bpm
        timeBetweenShooting = 60f / wd.gunStats.fireRate;
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
        muzzleFlash.GetComponent<WeaponMuzzleFlash>().AttachMuzzleFlash(muzzlePoint);

        //trigger event
        OnWeaponEquip?.Invoke(wid);

        shootSound = wd.soundClips.shootSound;
        bulletsLeft = wd.gunStats.magazineSize;
        timeBetweenShooting = 60f / wd.gunStats.fireRate;
    }

    private void MyInput()
    {
        if(wd.gunStats.allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < wd.gunStats.magazineSize && !inventory.switching) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0 && !inventory.switching)
        {
            bulletsShot = wd.gunStats.bulletsPerTap;
            Shoot();
        }

        if (reloading || bulletsLeft <= 0 || inventory.switching)
        {
            readyToShoot = false;
            canShoot = false;
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && !reloading)
            inventory.DropGun();
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float bulletSpread;
        if (aiming)
            bulletSpread = 0;
        else
        {
            //stand spread
            if (UIController.instance.crosshair.locomotionSpread == UIController.instance.crosshair.defaultSpread)
                bulletSpread = (UIController.instance.crosshair.bulletSpread + wd.gunStats.bulletAccuracy) / 1000;
            //crouch spread
            else if (UIController.instance.crosshair.locomotionSpread == UIController.instance.crosshair.crouchSpread)
                bulletSpread = (UIController.instance.crosshair.bulletSpread + wd.gunStats.bulletAccuracy) / 1200;
            else
                bulletSpread = UIController.instance.crosshair.spread / 500;
        }

        var spread = HenryUtilities.GetSpreadDirection(bulletSpread / 2, playerCam);

        Vector3 dir = (playerCam.transform.forward + spread).normalized;

        RaycastHit hit;
        Vector3 bulletDir;

        // Hitscan shoot
        if (Physics.Raycast(playerCam.transform.position, dir, out hit, wd.gunStats.bulletRange, whatIsShootable))
        {
            //hit damageable
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
            {
                hit.collider.GetComponentInParent<HealthManager>().TakeDamage(wd.gunStats.damage, wd.gunStats.bulletImpactForce, dir, hit.collider);
            }
            bulletDir = (hit.point - muzzlePoint.position).normalized;
            //Debug.DrawLine(playerCam.transform.position, hit.point, Color.red, 3.0f);
        }
        else
        {
            bulletDir = (HenryUtilities.GetDirectionToCenterScreen() + spread).normalized;
        }

        //render the bullet from the muzzle
        var bullet = new Bullet(muzzlePoint.position, bulletDir * wd.gunStats.bulletSpeed, true, this);
        bullets.Add(bullet);

        //muzzle flash
        muzzleFlash.Play();

        //bullet cast
        Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);

        //crosshair
        UIController.instance.crosshair.Fire();

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

    private void ResetShoot()
    {
        readyToShoot = true;
    }

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

        readyToShoot = true;
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
            //bullet impact hit
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Environment"))
                PoolManager.instance.SpawnGameObject(PoolTag.BulletImpact, hit.point, Quaternion.LookRotation(hit.normal));

            bullet.UpdateTracer(hit.point);
            bullet.time = maxLifetime;
        }
        else
        {
            bullet.UpdateTracer(end);
        }
    }

    public void CanShoot()
    {
        readyToShoot = true;
        canShoot = true;
    }
}
