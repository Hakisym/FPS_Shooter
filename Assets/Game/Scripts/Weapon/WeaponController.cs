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

    [Header("Reference")]
    public Camera playerCam;
    public Transform prefabHolder;
    public Transform muzzlePoint;
    public Transform castingPoint;
    public ParticleSystem muzzleFlash;
    public RaycastHit rayHit;
    public LayerMask whatIsShootable;
    public Animator animator;
    public CameraRecoilStacking cameraRecoil;
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

    class Bullet
    {
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public BulletType type;
        public enum BulletType
        {
            tracer,
            raycast,
            both
        }
        private TrailRenderer tracer;

        public Bullet(Vector3 position, Vector3 velocity, Bullet.BulletType type, WeaponController wc)
        {
            initialPosition = position;
            initialVelocity = velocity;
            time = 0.0f;
            this.type = type;
            AddTracer(wc.bulletTrail, position, wc.prefabHolder);
        }

        public void AddTracer(TrailRenderer tr, Vector3 position, Transform holder)
        {
            //bullet trail
            if (type != BulletType.raycast)
            {
                tracer = Instantiate(tr, position, Quaternion.identity, holder);
                tracer.AddPosition(position);
            }
        }

        public void UpdateTracer(Vector3 newPos)
        {
            //update tracer position
            if (tracer != null) tracer.transform.position = newPos;
        }

        public void OnDestroy()
        {
            //destroy bullet tracer after certain frame
            if (tracer != null) Destroy(tracer.gameObject, Time.deltaTime * 5f);
        }
    }

    private void Awake()
    {
        readyToShoot = true;
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
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && !reloading)
            inventory.DropGun();
    }

    private void Shoot()
    {
        readyToShoot = false;

        UIController.instance.crosshair.Fire();

        var bulletSpread = (UIController.instance.crosshair.spread - UIController.instance.crosshair.defaultSpread + wd.gunStats.bulletSpread) / 1000;
        //Spread
        float x = Random.Range(-bulletSpread / 2, bulletSpread / 2);
        float y = Random.Range(-bulletSpread / 2, bulletSpread / 2);
        Vector3 spreadDir = new Vector3(x, y, 0f);

        Vector3 directionToCenterScreen = GetDirectionToCenterScreen(muzzlePoint);

        //aim
        if (aiming)
        {
            //shoot the bullet from the muzzle
            var bullet = new Bullet(muzzlePoint.position, directionToCenterScreen * wd.gunStats.bulletSpeed, Bullet.BulletType.both, this);
            bullets.Add(bullet);
        }
        //hipfire
        else
        {
            var spread = playerCam.transform.TransformDirection(spreadDir);
            //render the trail
            var tracer = new Bullet(muzzlePoint.position, (directionToCenterScreen + spread).normalized * wd.gunStats.bulletSpeed, Bullet.BulletType.tracer, this);
            bullets.Add(tracer);

            //raycast the bullet
            var bullet = new Bullet(playerCam.transform.position, (playerCam.transform.forward + spread).normalized * wd.gunStats.bulletSpeed, Bullet.BulletType.raycast, this);
            bullets.Add(bullet);
        }

        //muzzle flash
        muzzleFlash.Play();

        //bullet cast
        Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation, prefabHolder);

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
        reloading = false;
    }

    private Vector3 GetPosition(Bullet bullet)
    {
        // p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * wd.gunStats.bulletDrop;
        return (bullet.initialPosition) + (bullet.initialVelocity * bullet.time) + (0.5f * bullet.time * bullet.time * gravity);
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
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        });
    }

    private void RaycastSegment(Vector3 start,Vector3 end,Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        //Raycast
        if (Physics.Raycast(start, direction, out rayHit, distance, whatIsShootable))
        {
            if (bullet.type != Bullet.BulletType.tracer)
            {
                //hit damageable
                if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
                {
                    rayHit.collider.GetComponentInParent<HealthManager>().TakeDamage(wd.gunStats.damage, wd.gunStats.bulletSpeed, direction, rayHit.collider);
                }
                //hit environment
                else
                {
                    //bullet impact
                    PoolManager.instance.SpawnGameObject(PoolTag.BulletImpact, rayHit.point, Quaternion.LookRotation(rayHit.normal));
                }
            }
            bullet.UpdateTracer(rayHit.point);
            bullet.time = maxLifetime;
        }
        else
        {
            bullet.UpdateTracer(end);
        }
    }

    Vector3 GetDirectionToCenterScreen(Transform muzzlePoint)
    {
        Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Ray ray = Camera.main.ScreenPointToRay(centerScreen);
        RaycastHit hit;

        // If the ray hits something, set direction towards hit point
        if (Physics.Raycast(ray, out hit))
        {
            return (hit.point - muzzlePoint.position).normalized;
        }
        else
        {
            // If ray does not hit anything, just return direction towards center of screen
            return (ray.direction).normalized;
        }
    }

    public void CanShoot() => readyToShoot = true;
}
