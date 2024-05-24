using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : WeaponMotionBase
{
    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 rot;

    private void Update()
    {
        //Return interpolation
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, wid.wd.weaponRecoilParameters.rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, wid.wd.weaponRecoilParameters.positionalReturnSpeed * Time.deltaTime);

        //Smooth lerp to current position
        target.localPosition = Vector3.Slerp(target.localPosition, positionalRecoil, wid.wd.weaponRecoilParameters.positionalRecoilSpeed * Time.deltaTime);
        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, rotationalRecoil, wid.wd.weaponRecoilParameters.rotationalRecoilSpeed * Time.deltaTime);
        target.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current position & rotation when firing
        if (wc.aiming)
        {
            rotationalRecoil += new Vector3(-wid.wd.weaponRecoilParameters.recoilRotationAim.x, 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotationAim.y, wid.wd.weaponRecoilParameters.recoilRotationAim.y), 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotationAim.z, wid.wd.weaponRecoilParameters.recoilRotationAim.z));

            positionalRecoil += new Vector3(Random.Range(-wid.wd.weaponRecoilParameters.recoilKickbackAim.x, wid.wd.weaponRecoilParameters.recoilKickbackAim.x),
                Random.Range(-wid.wd.weaponRecoilParameters.recoilKickbackAim.y, wid.wd.weaponRecoilParameters.recoilKickbackAim.y),
                wid.wd.weaponRecoilParameters.recoilKickbackAim.z);
        }
        else
        {
            rotationalRecoil += new Vector3(-wid.wd.weaponRecoilParameters.recoilRotation.x, 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotation.y, wid.wd.weaponRecoilParameters.recoilRotation.y), 
                Random.Range(-wid.wd.weaponRecoilParameters.recoilRotation.z, wid.wd.weaponRecoilParameters.recoilRotation.z));

            positionalRecoil += new Vector3(Random.Range(-wid.wd.weaponRecoilParameters.recoilKickback.x, wid.wd.weaponRecoilParameters.recoilKickback.x),
                Random.Range(-wid.wd.weaponRecoilParameters.recoilKickback.y, wid.wd.weaponRecoilParameters.recoilKickback.y),
                wid.wd.weaponRecoilParameters.recoilKickback.z);
        }
    }
}
