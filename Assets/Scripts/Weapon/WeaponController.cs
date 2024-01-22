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
    public bool shooting, reloading, aiming;
    private bool readyToShoot;

    [Header("Reference")]
    public Camera playerCam;
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
    public AudioSource audioSource;
    private AudioClip shootSound;

    public Vector3 shootDirection;
    private int bulletsLeft, bulletsShot;
    private Vector3 spreadAmount;

    private void Start()
    {
        bulletsLeft = wd.gunStats.magazineSize;
        readyToShoot = true;
        InitialSettings();
    }

    private void Update()
    {
        MyInput();
        if (!shooting || Mathf.Abs(Input.GetAxis("Mouse X")) > 1 || Mathf.Abs(Input.GetAxis("Mouse Y")) > 1)
        {
            spreadAmount = Vector3.zero;
        }
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
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-wd.gunStats.spread, wd.gunStats.spread);
        float y = Random.Range(0, wd.gunStats.spread);

        //Calculate direction with spread
        spreadAmount += new Vector3(x, y, 0f);
        shootDirection = playerCam.transform.forward + spreadAmount;

        //Raycast
        if (Physics.Raycast(playerCam.transform.position, shootDirection, out rayHit, wd.gunStats.range, whatIsShootable))
        {
            Debug.DrawLine(playerCam.transform.position, rayHit.point);
            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<IDamageable>().TakeDamage(wd.gunStats.damage);

            //bullet impact
            Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        }

        //Recoil
        cameraRecoil.Fire();
        weaponRecoil.Fire();

        //Graphics
        Instantiate(muzzleFlash, muzzlePoint.position, playerCam.transform.rotation,muzzlePoint);

        //Sounds
        audioSource.PlayOneShot(shootSound);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShoot", wd.gunStats.timeBetweenShooting);

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
        Invoke("ReloadFinished", wd.gunStats.reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = wd.gunStats.magazineSize;

        reloading = false;
    }

    private void InitialSettings()
    {
        playerCam = Camera.main;
        cameraRecoil = GetComponentInChildren<CameraRecoil>();
        weaponRecoil = GetComponentInChildren<WeaponRecoil>();
    }

    public void GetWeaponData(WeaponIdentification wid)
    {
        wd = wid.wd;
        muzzlePoint = wid.muzzle;
        shootSound = wd.audioSFX.firing;
    }
}
