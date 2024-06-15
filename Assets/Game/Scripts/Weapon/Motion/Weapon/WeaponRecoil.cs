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
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, wid.wd.weaponRecoilParameters.rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, wid.wd.weaponRecoilParameters.positionalReturnSpeed * Time.deltaTime);

        //Smooth lerp to current position
        target.localPosition = Vector3.Slerp(target.localPosition, positionalRecoil, wid.wd.weaponRecoilParameters.positionalRecoilSpeed * Time.fixedDeltaTime);
        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, rotationalRecoil, wid.wd.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime);
        target.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current position & rotation when firing
        //Recoil while aiming
        if (wc.aiming)
        {
            positionalRecoil += wid.wd.weaponRecoilParameters.staticRecoilPositionAim + new Vector3(Random.Range(-wid.wd.weaponRecoilParameters.recoilPositionAim.x, wid.wd.weaponRecoilParameters.recoilPositionAim.x),
                Random.Range(-wid.wd.weaponRecoilParameters.recoilPositionAim.y, wid.wd.weaponRecoilParameters.recoilPositionAim.y),
                Random.Range(-wid.wd.weaponRecoilParameters.recoilPositionAim.z, wid.wd.weaponRecoilParameters.recoilPositionAim.z));

            rotationalRecoil += wid.wd.weaponRecoilParameters.staticRecoilRotationAim + new Vector3(-wid.wd.weaponRecoilParameters.recoilRotationAim.x, 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotationAim.y, wid.wd.weaponRecoilParameters.recoilRotationAim.y));
        }
        //Recoil while not aiming
        else
        {
            positionalRecoil += wid.wd.weaponRecoilParameters.staticRecoilPosition + new Vector3(Random.Range(-wid.wd.weaponRecoilParameters.recoilPosition.x, wid.wd.weaponRecoilParameters.recoilPosition.x),
                Random.Range(-wid.wd.weaponRecoilParameters.recoilPosition.y, wid.wd.weaponRecoilParameters.recoilPosition.y),
                Random.Range(-wid.wd.weaponRecoilParameters.recoilPositionAim.z, wid.wd.weaponRecoilParameters.recoilPosition.z));

            rotationalRecoil += wid.wd.weaponRecoilParameters.staticRecoilRotation + new Vector3(wid.wd.weaponRecoilParameters.recoilRotation.x, 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotation.y, wid.wd.weaponRecoilParameters.recoilRotation.y), 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotation.z, wid.wd.weaponRecoilParameters.recoilRotation.z));
        }
    }
}
