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
    public RaycastHit rayHit;
    public LayerMask whatIsShootable;
    public Animator animator;
    public CameraRecoil cameraRecoil;
    public WeaponRecoil weaponRecoil;
    public Inventory inventory;

    [Header("Visuals")]
    public GameObject muzzleFlash;
    public GameObject bulletHoleGraphic;
    public GameObject bulletGO;

    [Header("Sounds")]
    public AudioSource audioSource;
    private AudioClip shootSound;

    private int bulletsLeft, bulletsShot;

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

        var direction = playerCam.transform.forward + new Vector3(x, y, 0f);

        //Raycast
        if (Physics.Raycast(playerCam.transform.position, direction, out rayHit, wd.gunStats.range, whatIsShootable))
        {
            //hit damage
            if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                rayHit.collider.GetComponentInParent<IDamageable>().TakeDamage(wd.gunStats.damage,wd.gunStats.bulletForce, direction, rayHit.collider);
            }
            else
            {
                //bullet impact
                Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
            }
        }

        //Recoil
        cameraRecoil.Fire();
        weaponRecoil.Fire();

        //Graphics
        Instantiate(muzzleFlash, muzzlePoint.position, playerCam.transform.rotation,muzzlePoint);
        var bulletGO = Instantiate(this.bulletGO, muzzlePoint.position, playerCam.transform.rotation);
        bulletGO.GetComponent<Rigidbody>().AddForce(bulletGO.transform.forward*100f, ForceMode.Impulse);

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

    }

    public void GetWeaponData(WeaponIdentification wid)
    {
        wd = wid.wd;
        muzzlePoint = wid.muzzle;
        shootSound = wd.audioSFX.firing;
        bulletsLeft = wd.gunStats.magazineSize;
    }
}
