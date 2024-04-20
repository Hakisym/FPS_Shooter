using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Gun Settings")]
    public string gunName;
    public GameObject gunPrefab;
    public GameObject pickUpGraphics;
    public GunStats gunStats;

    [Header("Recoil Settings")]
    public WeaponRecoilParameters weaponRecoilParameters;
    public CameraRecoilParameters cameraRecoilParameters;

    [Header("Visual")]
    public float camShakeIntensity;

    [Header("Audio")]
    public AudioClips soundClips;

    [Header("UI")]
    public float crosshairResize;
    public float crosshairResizeSpeed;

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
        public AudioClip shootSound;
        public AudioClip holsterSound;
        public AudioClip takeOutSound;
        public AudioClip reloadSoundAmmoLeft;
        public AudioClip reloadSoundOutOfAmmo;
        public AudioClip emptyMagShoot;
        public AudioClip aimSound;
    }
}


