using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Recoil Settings")]
    public WeaponRecoilParameters weaponRecoilParameters;

    [Header("Camera Recoil Settings")]
    public CameraRecoilParameters cameraRecoilParameters;

    [Header("Aim Settings")]
    public AimParameters aimParameters;

    [Header("Gun stats")]
    public string gunName;
    public GameObject gunPrefab;
    public GameObject pickUpGraphics;
    public GunStats gunStats;

    [Header("Audio")]
    public AudioClips audioSFX;
}

[System.Serializable]
public struct WeaponRecoilParameters
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
public struct CameraRecoilParameters
{
    public Vector3 RecoilRotation; //hipfire
    public Vector3 RecoilRotationAiming; //aiming
    //speed
    public float rotationSpeed;
    public float returnSpeed;
}

[System.Serializable]
public struct AimParameters
{
    public Vector3 aimPosition;
    public Vector3 aimRotation;
    public float smoothTime;
    public float smoothRotation;
}

[System.Serializable]
public struct GunStats
{
    public int damage,bulletForce;
    public float fireRate, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
}

[System.Serializable]
public struct AudioClips
{
    public AudioClip firing, hoster, unhoster, reload, emptyMagReload, emptyMagShoot;
}
