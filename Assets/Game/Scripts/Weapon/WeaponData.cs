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

    [Header("Aim Settings")]
    public AimParameters aimParameters;

    [Header("Recoil Settings")]
    public WeaponRecoilParameters weaponRecoilParameters;
    public CameraRecoilParameters cameraRecoilParameters;

    [Header("Audio")]
    public AudioClips soundClips;

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
        public float recoilAmountX;
        public float recoilAmountY;
    }

    [System.Serializable]
    public class AimParameters
    {
        public Vector3 aimPosition;
        public Vector3 aimRotation;
        public float smoothTime;
        public float smoothRotation;
    }

    [System.Serializable]
    public class GunStats
    {
        public int damage, bulletForce;
        public float fireRate, spread, range, reloadTime, timeBetweenShots;
        public int magazineSize, bulletsPerTap;
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


