using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Reference")]
    public Transform recoilPosition;
    public Transform rotationPoint;
    public WeaponController wc;
    public Inventory inventory;

    private Vector3 rotationalRecoil;
    private Vector3 positionalRecoil;
    private Vector3 rot;

    private void OnEnable()
    {
        wc.OnWeaponEquip += HandleWeaponEquip;
    }

    private void OnDisable()
    {
        wc.OnWeaponEquip -= HandleWeaponEquip;
    }

    private void FixedUpdate()
    {
        //Return interpolation
        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, inventory.currentWeapon.wd.weaponRecoilParameters.rotationalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, inventory.currentWeapon.wd.weaponRecoilParameters.positionalReturnSpeed * Time.deltaTime);

        //Smooth lerp to current position
        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, inventory.currentWeapon.wd.weaponRecoilParameters.positionalRecoilSpeed * Time.fixedDeltaTime);
        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, rotationalRecoil, inventory.currentWeapon.wd.weaponRecoilParameters.rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        //Add random offset to current position & rotation when firing
        if (wc.aiming)
        {
            rotationalRecoil += new Vector3(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotationAim.x, 
                Random.Range(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotationAim.y, inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotationAim.y), 
                Random.Range(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotationAim.z, inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotationAim.z));

            positionalRecoil += new Vector3(Random.Range(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickbackAim.x, inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickbackAim.x),
                inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickbackAim.y,
                inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickbackAim.z);
        }
        else
        {
            rotationalRecoil += new Vector3(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotation.x, 
                Random.Range(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotation.y, inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotation.y), 
                Random.Range(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotation.z, inventory.currentWeapon.wd.weaponRecoilParameters.recoilRotation.z));

            positionalRecoil += new Vector3(Random.Range(-inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickback.x, inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickback.x),
                inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickback.y,
                inventory.currentWeapon.wd.weaponRecoilParameters.recoilKickback.z);
        }
    }

    private void HandleWeaponEquip(WeaponIdentification wid)
    {
        rotationPoint = wid.root;
    }
}
