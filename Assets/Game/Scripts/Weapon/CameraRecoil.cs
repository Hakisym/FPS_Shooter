using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Reference")]
    public WeaponController wc;
    public Inventory inventory;
    public Camera playerCam;

    private Vector3 currentRotation;
    private Vector3 rot;

    private void FixedUpdate()
    {
        //Return to (0,0,0) after continuous fire
        Vector3 returnPoint = Input.GetMouseButton(0) ? new Vector3(currentRotation.x, 0f, 0f) : Vector3.zero;

        //Return interpolation
        currentRotation = Vector3.Lerp(currentRotation, returnPoint, inventory.currentWeapon.wd.cameraRecoilParameters.returnSpeed * Time.deltaTime);

        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, currentRotation, inventory.currentWeapon.wd.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current rotation when firing
        if (wc.aiming)
        {
            currentRotation += new Vector3(-inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotationAiming.x,
                Random.Range(-inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotationAiming.y, inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotationAiming.y),
                Random.Range(-inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotationAiming.z, inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotationAiming.z));
        }
        else
        {
            currentRotation += new Vector3(-inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotation.x,
                Random.Range(-inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotation.y, inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotation.y),
                Random.Range(-inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotation.z, inventory.currentWeapon.wd.cameraRecoilParameters.RecoilRotation.z));
        }
    }
}
