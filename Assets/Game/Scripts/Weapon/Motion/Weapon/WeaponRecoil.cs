using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : WeaponMotionBase
{
    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 rot;

    private void FixedUpdate()
    {
        //Return interpolation
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, wid.preset.weaponRecoilParameters.rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, wid.preset.weaponRecoilParameters.positionalReturnSpeed * Time.deltaTime);

        //Smooth lerp to current position
        target.localPosition = Vector3.Slerp(target.localPosition, positionalRecoil, wid.preset.weaponRecoilParameters.positionalRecoilSpeed * Time.fixedDeltaTime);
        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, rotationalRecoil, wid.preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime);
        target.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current position & rotation when firing
        //Recoil while aiming
        if (wc.aiming)
        {
            positionalRecoil += wid.preset.weaponRecoilParameters.staticRecoilPositionAim + new Vector3(Random.Range(-wid.preset.weaponRecoilParameters.recoilPositionAim.x, wid.preset.weaponRecoilParameters.recoilPositionAim.x),
                Random.Range(-wid.preset.weaponRecoilParameters.recoilPositionAim.y, wid.preset.weaponRecoilParameters.recoilPositionAim.y),
                Random.Range(-wid.preset.weaponRecoilParameters.recoilPositionAim.z, wid.preset.weaponRecoilParameters.recoilPositionAim.z));

            rotationalRecoil += wid.preset.weaponRecoilParameters.staticRecoilRotationAim + new Vector3(-wid.preset.weaponRecoilParameters.recoilRotationAim.x, 
                Random.Range(-wid.preset.weaponRecoilParameters.recoilRotationAim.y, wid.preset.weaponRecoilParameters.recoilRotationAim.y));
        }
        //Recoil while not aiming
        else
        {
            positionalRecoil += wid.preset.weaponRecoilParameters.staticRecoilPosition + new Vector3(Random.Range(-wid.preset.weaponRecoilParameters.recoilPosition.x, wid.preset.weaponRecoilParameters.recoilPosition.x),
                Random.Range(-wid.preset.weaponRecoilParameters.recoilPosition.y, wid.preset.weaponRecoilParameters.recoilPosition.y),
                Random.Range(-wid.preset.weaponRecoilParameters.recoilPositionAim.z, wid.preset.weaponRecoilParameters.recoilPosition.z));

            rotationalRecoil += wid.preset.weaponRecoilParameters.staticRecoilRotation + new Vector3(wid.preset.weaponRecoilParameters.recoilRotation.x, 
                Random.Range(-wid.preset.weaponRecoilParameters.recoilRotation.y, wid.preset.weaponRecoilParameters.recoilRotation.y), 
                Random.Range(-wid.preset.weaponRecoilParameters.recoilRotation.z, wid.preset.weaponRecoilParameters.recoilRotation.z));
        }
    }
}
