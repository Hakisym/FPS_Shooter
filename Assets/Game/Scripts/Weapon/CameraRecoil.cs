using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Reference")]
    public WeaponController wc;

    private Vector3 currentRotation, rot;
    private float evaluationProgress, evaluationProgressX;

    private void FixedUpdate()
    {
        if (!wc.wd) return;

        if (!wc.shootState || !wc.canShoot)
        {
            evaluationProgress = 0;
            evaluationProgressX = 0;
            //Return interpolation
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, wc.wd.cameraRecoilParameters.returnSpeed * Time.fixedDeltaTime);
        }

        if (wc.canShoot && wc.shootState)
            if (wc.wd.cameraRecoilParameters.type == RecoilType.Curve)
            {
                var xRecoil = wc.aiming ? wc.wd.cameraRecoilParameters.xRecoilAmount * 2 : wc.wd.cameraRecoilParameters.xRecoilAmount;
                var yRecoil = wc.aiming ? wc.wd.cameraRecoilParameters.yRecoilAmount * 2 : wc.wd.cameraRecoilParameters.yRecoilAmount;
                currentRotation = new Vector3(-wc.wd.cameraRecoilParameters.recoilY.Evaluate(evaluationProgress) * yRecoil, -wc.wd.cameraRecoilParameters.recoilX.Evaluate(evaluationProgress) * xRecoil, 0);
            }

        //Smooth rotate to current rotation
        rot = Vector3.Slerp(rot, currentRotation, wc.wd.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Fire()
    {
        if (wc.wd.cameraRecoilParameters.type == RecoilType.Procedural)
        {
            //Add random offset to current rotation when firing
            if (wc.aiming)
            {
                currentRotation += new Vector3(-wc.wd.cameraRecoilParameters.recoilRotationAiming.x,
                    Random.Range(-wc.wd.cameraRecoilParameters.recoilRotationAiming.y, wc.wd.cameraRecoilParameters.recoilRotationAiming.y),
                    Random.Range(-wc.wd.cameraRecoilParameters.recoilRotationAiming.z, wc.wd.cameraRecoilParameters.recoilRotationAiming.z));
            }
            else
            {
                currentRotation += new Vector3(-wc.wd.cameraRecoilParameters.recoilRotation.x,
                    Random.Range(-wc.wd.cameraRecoilParameters.recoilRotation.y, wc.wd.cameraRecoilParameters.recoilRotation.y),
                    Random.Range(-wc.wd.cameraRecoilParameters.recoilRotation.z, wc.wd.cameraRecoilParameters.recoilRotation.z));
            }
        }
        else
        {
            evaluationProgress += 1f / wc.wd.gunStats.magazineSize;
            evaluationProgressX += 1f / wc.wd.gunStats.magazineSize;
        }
    }
}

public enum RecoilType
{
    Procedural,
    Curve
}