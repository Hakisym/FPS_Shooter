using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6f;
    public float returnSpeed = 6f;

    [Header("Hipfire")]
    public Vector3 recoilRotation = new Vector3(2f, 2f, 2f);

    [Header("Aiming")]
    public Vector3 recoilRotationAiming = new Vector3(0.5f, 0.5f, 1.5f);

    [Header("Reference")]
    public PlayerBehaviour pb;

    private Vector3 currentRotation;
    private Vector3 rot;

    private void FixedUpdate()
    {
        //Return to (0,0,0) after continuous fire
        Vector3 returnPoint = Input.GetMouseButton(0) ? new Vector3(currentRotation.x, 0f, 0f) : Vector3.zero;

        //Return interpolation
        currentRotation = Vector3.Lerp(currentRotation, returnPoint, returnSpeed * Time.deltaTime);
        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current rotation when firing
        if (pb.isAiming)
        {
            currentRotation += new Vector3(-recoilRotationAiming.x, Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y), Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
        }
        else
        {
            currentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
        }
    }
}
