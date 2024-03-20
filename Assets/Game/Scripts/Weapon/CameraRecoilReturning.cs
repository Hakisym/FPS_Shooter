using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilReturning : MonoBehaviour
{
    [Header("Reference")]
    public WeaponController wc;
    public Inventory inventory;
    public Camera playerCam;

    public Vector3 recoilRotation; //hipfire
    public Vector3 recoilRotationAiming; //aiming
    //speed
    public float rotationSpeed;
    public float returnSpeed;

    private Vector3 rot;
    private Vector3 currentRotation;

    private void FixedUpdate()
    {
        //Return interpolation
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);

        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current rotation when firing
        if (wc.aiming)
        {
            currentRotation += new Vector3(-recoilRotationAiming.x,
                Random.Range(-recoilRotationAiming.y, recoilRotationAiming.y),
                Random.Range(-recoilRotationAiming.z, recoilRotationAiming.z));
        }
        else
        {
            currentRotation += new Vector3(-recoilRotation.x,
                Random.Range(-recoilRotation.y, recoilRotation.y),
                Random.Range(-recoilRotation.z, recoilRotation.z));
        }
    }
}