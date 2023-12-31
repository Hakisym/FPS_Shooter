using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunSystem : MonoBehaviour
{
    [Header("Gun stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    [Header("Reference")]
    public Camera fpsCam;
    public Transform muzzlePoint;
    public RaycastHit rayHit;
    public LayerMask whatIsShootable;

    // Graphics
    public GameObject muzzleFlash, bulletHoleGraphic;
    public CameraRecoil cameraRecoil;
    public WeaponRecoil weaponRecoil;

    private void Start()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    void MyInput()
    {
        if(allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < magazineSize) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate direction with spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0f);

        //Raycast
        if(Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsShootable))
        {
            Debug.DrawLine(fpsCam.transform.position, rayHit.point);
            if (rayHit.collider.CompareTag("Enemy"))
                rayHit.collider.GetComponent<IDamageable>().TakeDamage(damage);

            //bullet impact
            Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
        }

        //Recoil
        cameraRecoil.Fire();
        weaponRecoil.Fire();

        //Graphics
        Instantiate(muzzleFlash, muzzlePoint.position, fpsCam.transform.rotation,muzzlePoint);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShoot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

    void ResetShoot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    void ReloadFinished()
    {
        bulletsLeft = magazineSize;

        reloading = false;
    }
}
