using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : WeaponMotionBase
{
    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 rot;
    private Vector3 pos;
    private WeaponData preset;

    public override void Setup(IWeaponStateProvider provider)
    {
        base.Setup(provider);
        preset = stateProvider.Preset;
    }

    private void FixedUpdate()
    {
        //Return interpolation
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, preset.weaponRecoilParameters.rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, preset.weaponRecoilParameters.positionalReturnSpeed * Time.deltaTime);

        //Smooth lerp to current position
        //target.localPosition = Vector3.Slerp(target.localPosition, positionalRecoil, wid.preset.weaponRecoilParameters.positionalRecoilSpeed * Time.fixedDeltaTime);
        pos = new Vector3(
            Mathf.Lerp(pos.x, positionalRecoil.x, preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime),
            Mathf.Lerp(pos.y, positionalRecoil.y, preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime),
            Mathf.Lerp(pos.z, positionalRecoil.z, preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime));

        //Smooth rotate to current rotation
        //rot = Vector3.Slerp(rot, rotationalRecoil, wid.preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime);
        rot = new Vector3(
            Mathf.Lerp(rot.x, rotationalRecoil.x, preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime),
            Mathf.Lerp(rot.y, rotationalRecoil.y, preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime),
            Mathf.Lerp(rot.z, rotationalRecoil.z, preset.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime));

        target.localPosition = pos;
        target.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current position & rotation when firing
        //Recoil while aiming
        if (stateProvider.IsAiming)
        {
            positionalRecoil += preset.weaponRecoilParameters.staticRecoilPositionAim + new Vector3(Random.Range(-preset.weaponRecoilParameters.recoilPositionAim.x, preset.weaponRecoilParameters.recoilPositionAim.x),
                Random.Range(-preset.weaponRecoilParameters.recoilPositionAim.y, preset.weaponRecoilParameters.recoilPositionAim.y),
                Random.Range(-preset.weaponRecoilParameters.recoilPositionAim.z, preset.weaponRecoilParameters.recoilPositionAim.z));

            rotationalRecoil += preset.weaponRecoilParameters.staticRecoilRotationAim + new Vector3(Random.Range(-preset.weaponRecoilParameters.recoilRotationAim.x, 0),
                Random.Range(-preset.weaponRecoilParameters.recoilRotationAim.y, preset.weaponRecoilParameters.recoilRotationAim.y),
                Random.Range(-preset.weaponRecoilParameters.recoilRotationAim.z, preset.weaponRecoilParameters.recoilRotationAim.z));
        }
        //Recoil while not aiming
        else
        {
            positionalRecoil += preset.weaponRecoilParameters.staticRecoilPosition + new Vector3(Random.Range(-preset.weaponRecoilParameters.recoilPosition.x, preset.weaponRecoilParameters.recoilPosition.x),
                Random.Range(-preset.weaponRecoilParameters.recoilPosition.y, preset.weaponRecoilParameters.recoilPosition.y),
                Random.Range(-preset.weaponRecoilParameters.recoilPositionAim.z, preset.weaponRecoilParameters.recoilPosition.z));

            rotationalRecoil += preset.weaponRecoilParameters.staticRecoilRotation + new Vector3(Random.Range(-preset.weaponRecoilParameters.recoilRotation.x, 0),
                Random.Range(-preset.weaponRecoilParameters.recoilRotation.y, preset.weaponRecoilParameters.recoilRotation.y), 
                Random.Range(-preset.weaponRecoilParameters.recoilRotation.z, preset.weaponRecoilParameters.recoilRotation.z));
        }
    }
}
