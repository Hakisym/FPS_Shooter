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
    private bool readyToShoot;

    [Header("Reference")]
    public Camera playerCam;
    public Transform muzzlePoint;
    public Transform castingPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsShootable;
    public Animator animator;
    public CameraRecoilStacking cameraRecoil;
    public WeaponRecoil weaponRecoil;
    public Inventory inventory;

    [Header("Visuals")]
    public ParticleSystem muzzleFlash;
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

    private void Start()
    {
        GetWeaponData(inventory.currentWeapon);
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
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

        if (Input.GetKeyDown(KeyCode.Backspace) && !reloading)
            inventory.DropGun();
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-wd.gunStats.spread, wd.gunStats.spread);
        float y = Random.Range(0, wd.gunStats.spread);
        Vector3 spreadDir = new Vector3(x, y, 0f);

        var direction = muzzlePoint.forward;

        rayHit = new RaycastHit();
        //Raycast
        if (Physics.Raycast(muzzlePoint.position, direction, out rayHit, wd.gunStats.range, whatIsShootable))
        {
            //hit damageable
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("EnemyHit"))
            {
                rayHit.collider.GetComponentInParent<HealthManager>().TakeDamage(wd.gunStats.damage,wd.gunStats.bulletForce, direction, rayHit.collider);
            }
            //hit environment
            else
            {
                //bullet impact
                Instantiate(bulletImpact, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
            //Debug.DrawLine(muzzlePoint.position, rayHit.point, Color.red, 1.0f);
        }

        //bullet trail
        var tracer = Instantiate(bulletTrail, muzzlePoint.position, Quaternion.identity);
        tracer.AddPosition(muzzlePoint.position);
        tracer.transform.position = rayHit.point;

        //muzzle flash
        muzzleFlash.Play();

        //bullet cast
        Instantiate(bulletCasting, castingPoint.position, castingPoint.rotation);

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

        reloading = false;
    }

    public void GetWeaponData(WeaponIdentification wid)
    {
        wd = wid.wd;
        muzzlePoint = wid.muzzle;
        castingPoint = wid.castPoint;
        //reset muzzle position
        muzzleFlash.transform.SetParent(muzzlePoint);
        muzzleFlash.transform.localPosition = Vector3.zero;

        shootSound = wd.soundClips.shootSound;
        bulletsLeft = wd.gunStats.magazineSize;
        timeBetweenShooting = 60f / wd.gunStats.fireRate;
    }
}
