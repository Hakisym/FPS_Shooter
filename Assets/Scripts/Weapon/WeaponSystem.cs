using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponSystem : MonoBehaviour
{
    [Header("WeaponData")]
    public WeaponData wd;

    [Header("Gun states")]
    public bool shooting, readyToShoot, reloading;

    [Header("Reference")]
    public Camera fpsCam;
    public Transform muzzlePoint;
    public RaycastHit rayHit;
    public LayerMask whatIsShootable;
    public Animator animator;
    public CameraRecoil cameraRecoil;
    public WeaponRecoil weaponRecoil;
    public Inventory inventory;

    [Header("Visuals")]
    public GameObject muzzleFlash, bulletHoleGraphic;

    [Header("Sounds")]
    public AudioClip shootSound;
    public AudioSource audioSource;

    int bulletsLeft, bulletsShot;

    private void Start()
    {
        bulletsLeft = wd.gunStats.magazineSize;
        readyToShoot = true;
        InitialSettings();
    }

    private void Update()
    {
        MyInput();
    }

    void MyInput()
    {
        if(wd.gunStats.allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < wd.gunStats.magazineSize && !inventory.switching) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0 && !inventory.switching)
        {
            bulletsShot = wd.gunStats.bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-wd.gunStats.spread, wd.gunStats.spread);
        float y = Random.Range(-wd.gunStats.spread, wd.gunStats.spread);

        //Calculate direction with spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0f);

        //Raycast
        if(Physics.Raycast(fpsCam.transform.position, direction, out rayHit, wd.gunStats.range, whatIsShootable))
        {
            Debug.DrawLine(fpsCam.transform.position, rayHit.point);
            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<IDamageable>().TakeDamage(wd.gunStats.damage);

            //bullet impact
            Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        }

        //Recoil
        cameraRecoil.Fire();
        weaponRecoil.Fire();

        //Graphics
        Instantiate(muzzleFlash, muzzlePoint.position, fpsCam.transform.rotation,muzzlePoint);

        //Sounds
        audioSource.PlayOneShot(shootSound);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShoot", wd.gunStats.timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", wd.gunStats.timeBetweenShots);
    }

    void ResetShoot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        animator.SetTrigger("reloading");
        Invoke("ReloadFinished", wd.gunStats.reloadTime);
    }

    void ReloadFinished()
    {
        bulletsLeft = wd.gunStats.magazineSize;

        reloading = false;
    }

    private void InitialSettings()
    {
        fpsCam = Camera.main;
        animator = GetComponentInParent<Animator>();
        inventory = GetComponentInParent<Inventory>();
        var player = GameObject.FindGameObjectWithTag("Player");
        cameraRecoil = player.GetComponentInChildren<CameraRecoil>();
        weaponRecoil = player.GetComponentInChildren<WeaponRecoil>();
    }
}
