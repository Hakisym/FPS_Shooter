using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Recoil Settings")]
    public Vector3 weaponRecoilRotation = new Vector3(10f, 5f, 7f);
    public Vector3 weaponRecoilKickback = new Vector3(0.015f, 0f, -0.2f);

    public Vector3 weaponRecoilRotationAim = new Vector3(10f, 4f, 6f);
    public Vector3 weaponRecoilKickbackAim = new Vector3(0.015f, 0f, -0.2f);

    [Header("Camera Recoil Settings")]
    public Vector3 cameraRecoilRotation = new Vector3(2f, 2f, 2f);
    public Vector3 cameraRecoilRotationAiming = new Vector3(0.5f, 0.5f, 1.5f);

    [Header("Aim Offsets")]
    public Vector3 aimPosition;
    public Vector3 aimRotation;
}
