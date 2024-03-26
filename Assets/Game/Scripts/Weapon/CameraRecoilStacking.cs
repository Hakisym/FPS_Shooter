using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilStacking : MonoBehaviour
{
    
    public WeaponController wc;
    public Inventory inventory;
    public MouseLook mls;

    private float currentRecoilXPos;
    private float currentRecoilYPos;
    
    public void Fire()
    {
        currentRecoilXPos = wc.aiming ? ((Random.value - .5f) / 2) * inventory.currentWeapon.wd.cameraRecoilParameters.recoilAmountX : 0f;
        currentRecoilYPos = 0.25f * inventory.currentWeapon.wd.cameraRecoilParameters.recoilAmountY;
        mls.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos);
        mls.wantedYRotation -= currentRecoilXPos;
    }
}
