using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilStacking : CameraRecoilBase
{
    public float recoilAmountX;
    public float recoilAmountY;
    public MouseLook mls;

    private float currentRecoilXPos;
    private float currentRecoilYPos;
    
    public override void Fire()
    {
        currentRecoilXPos = ((Random.value - .5f) / 2) * recoilAmountX;
        currentRecoilYPos = 0.25f * recoilAmountY;
        mls.wantedCameraXRotation -= Mathf.Abs(currentRecoilYPos);
        mls.wantedYRotation -= currentRecoilXPos;
    }
}
