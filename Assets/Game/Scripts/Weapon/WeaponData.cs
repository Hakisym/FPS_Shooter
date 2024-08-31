using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Gun Settings")]
    public int gunID;
    public string gunName;
    public GameObject gunPrefab;
    public GameObject gunModel;
    public GameObject pickUpGraphics;
    public GunStats gunStats;

    [Header("Recoil Settings")]
    public WeaponRecoilParameters weaponRecoilParameters;
    public CameraRecoilParameters cameraRecoilParameters;

    [Header("Camera")]
    public float cameraShake;
    public float cameraFadeoutTime;

    [Header("Visual")]
    public GameObject bulletCasting;

    [Header("Audio")]
    public AudioClips soundClips;

    [Header("Third Person")]
    public ThirdPersonData thirdPersonData;

    [System.Serializable]
    public struct WeaponRecoilParameters
    {
        public Vector3 staticRecoilPosition;
        public Vector3 staticRecoilRotation;

        public Vector3 staticRecoilPositionAim;
        public Vector3 staticRecoilRotationAim;

        [Space]
        public Vector3 recoilPosition;
        public Vector3 recoilRotation;

        public Vector3 recoilPositionAim;
        public Vector3 recoilRotationAim;

        [Space]
        public float positionalRecoilSpeed;
        public float rotationalRecoilSpeed;

        public float positionalReturnSpeed;
        public float rotationalReturnSpeed;
    }

    [System.Serializable]
    public struct CameraRecoilParameters
    {
        public float rotationSpeed, returnSpeed, returnDuration;

        [Header("Procedural Recoil")]
        public Vector3 recoilRotation;
        public Vector3 recoilRotationAiming;

        [Header("Curve Recoil")]
        public AnimationCurve recoilX;
        public AnimationCurve recoilY;
        public float xRecoilAmount, yRecoilAmount;
    }

    [System.Serializable]
    public struct GunStats
    {
        [Header("General")]
        public int damage;
        public int magazineSize;
        public float fireRate;
        public float reloadTime;
        public float adsTime;
        [Header("Bullet")]
        public float impactForce;
        public float velocity;
        public float bulletDrop;
        public float range;
        [Header("Shoot Mode")]
        public ShootMode fireMode;
        public int bulletsPerTap;
        public int pelletsPerBullet;
        public float timeBetweenShots;
        public float chamberTime;
        [Header("Spread")]
        public float defaultSpread;
        public float crouchSpread;
        public float walkSpread;
        public float runSpread;
        public float jumpSpread;
        public float bulletSpreadOneShot;
        public float bulletSpreadResizeSpeed;
        public float aimSpreadMultiplier;
    }

    [System.Serializable]
    public struct AudioClips
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

    [System.Serializable]
    public struct ThirdPersonData
    {
        public Vector3 ikHandPos;
        public Vector3 rightHandTarget;
        public Vector3 rightHandTargetAim;
        public Vector3 leftHandTarget;
    }
}

public enum ShootMode
{
    Semi,
    Auto,
    PumpAction
}


