using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Reference")]
    public Transform recoilPosition;
    public Transform rotationPoint;
    public PlayerBehaviour pb;

    [Header("Speed Settings")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;

    public float positionalReturnSpeed = 18f;
    public float rotationalReturnSpeed = 38f;

    [Header("Amount Settings")]
    public Vector3 recoilRotation = new Vector3(10f, 5f, 7f);
    public Vector3 recoilKickback = new Vector3(0.015f, 0f, -0.2f);

    public Vector3 recoilRotationAim = new Vector3(10f, 4f, 6f);
    public Vector3 recoilKickbackAim = new Vector3(0.015f, 0f, -0.2f);

    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 rot;

    private void FixedUpdate()
    {
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, positionalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, positionalRecoilSpeed * Time.fixedDeltaTime);
        rot = Vector3.Slerp(rot, rotationalRecoil, rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        if (pb.isAiming)
        {
            rotationalRecoil += new Vector3(-recoilRotationAim.x, Random.Range(-recoilRotationAim.y, recoilRotationAim.y), Random.Range(-recoilRotationAim.z, recoilRotationAim.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickbackAim.x, recoilKickbackAim.x), Random.Range(-recoilKickbackAim.y, recoilKickbackAim.y), recoilKickbackAim.z);
        }
        else
        {
            rotationalRecoil += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
            positionalRecoil += new Vector3(Random.Range(-recoilKickback.x, recoilKickback.x), Random.Range(-recoilKickback.y, recoilKickback.y), recoilKickback.z);
        }
    }
}
