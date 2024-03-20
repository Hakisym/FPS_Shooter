using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilReturning : CameraRecoilBase
{
    [Header("Reference")]
    public WeaponController wc;
    public Inventory inventory;
    public Camera playerCam;

    private Vector3 rot;
    private Vector3 currentRotation;

    private void FixedUpdate()
    {
        //Return interpolation
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, inventory.currentWeapon.wd.cameraRecoilParameters.returnSpeed * Time.deltaTime);

        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, currentRotation, inventory.currentWeapon.wd.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public override void Fire()
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

public abstract class CameraRecoilBase : MonoBehaviour
{
    public abstract void Fire();
}
