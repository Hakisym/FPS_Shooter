using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Gun Settings")]
    public string gunName;
    public GameObject gunPrefab;
    public GameObject gunModel;
    public GameObject pickUpGraphics;
    public GunStats gunStats;

    [Header("Recoil Settings")]
    public WeaponRecoilParameters weaponRecoilParameters;
    public CameraRecoilParameters cameraRecoilParameters;
    public float camShakeIntensity;

    [Header("Visual")]
    public GameObject bulletCasting;

    [Header("Audio")]
    public AudioClips soundClips;

    [Header("UI")]
    public float crosshairResize;
    public float crosshairResizeSpeed;

    [Header("Third Person")]
    public Vector3 ikHandPos;

    [System.Serializable]
    public class WeaponRecoilParameters
    {
        //hipfire
        public Vector3 recoilRotation;
        public Vector3 recoilKickback;
        //aiming
        public Vector3 recoilRotationAim;
        public Vector3 recoilKickbackAim;
        //speed
        public float positionalRecoilSpeed;
        public float rotationalRecoilSpeed;

        public float positionalReturnSpeed;
        public float rotationalReturnSpeed;
    }

    [System.Serializable]
    public class CameraRecoilParameters
    {
        public RecoilType type;
        public float rotationSpeed, returnSpeed;

        [Header("Procedural Recoil")]
        public Vector3 recoilRotation;
        public Vector3 recoilRotationAiming;

        [Header("Curve Recoil")]
        public AnimationCurve recoilX;
        public AnimationCurve recoilY;
        public float xRecoilAmount, yRecoilAmount;
    }

    [System.Serializable]
    public class GunStats
    {
        [Header("General")]
        public int damage;
        public int magazineSize;
        public float fireRate;
        public float reloadTime;
        [Header("Bullet")]
        public float bulletImpactForce;
        public float bulletSpeed;
        public float bulletDrop;
        public float bulletRange;
        public float bulletAccuracy;
        [Header("Shoot Mode")]
        public int bulletsPerTap;
        public float timeBetweenShots;
        [Header("Misc")]
        public bool allowButtonHold;
    }

    [System.Serializable]
    public class AudioClips
    {
        public AudioProfile shootSound;
        public AudioProfile fireTail;
        public AudioProfile holsterSound;
        public AudioProfile takeOutSound;
        public AudioProfile reloadSoundAmmoLeft;
        public AudioProfile reloadSoundOutOfAmmo;
        public AudioProfile emptyMagShoot;
        public AudioProfile aimSound;
    }
}


