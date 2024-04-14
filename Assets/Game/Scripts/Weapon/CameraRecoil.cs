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
        if (!wc.shooting || !wc.canShoot)
        {
            if (wc.wd.cameraRecoilParameters.type == RecoilType.Curve)
            {
                evaluationProgress = Mathf.Lerp(evaluationProgress,0, wc.wd.cameraRecoilParameters.returnSpeed * Time.fixedDeltaTime);
                evaluationProgressX = Mathf.Lerp(evaluationProgress, 0, wc.wd.cameraRecoilParameters.returnSpeed * Time.fixedDeltaTime);
                currentRotation = new Vector3(-wc.wd.cameraRecoilParameters.recoilY.Evaluate(evaluationProgress) * wc.wd.cameraRecoilParameters.yRecoilAmount, -wc.wd.cameraRecoilParameters.recoilX.Evaluate(evaluationProgress) * wc.wd.cameraRecoilParameters.xRecoilAmount, 0);
            }
            else
            {
                //Return interpolation
                currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, wc.wd.cameraRecoilParameters.returnSpeed * Time.fixedDeltaTime);
            }
        }

        if (wc.shooting && wc.canShoot)
            if (wc.wd.cameraRecoilParameters.type == RecoilType.Curve)
                currentRotation = new Vector3(-wc.wd.cameraRecoilParameters.recoilY.Evaluate(evaluationProgress) * wc.wd.cameraRecoilParameters.yRecoilAmount, -wc.wd.cameraRecoilParameters.recoilX.Evaluate(evaluationProgress) * wc.wd.cameraRecoilParameters.xRecoilAmount, 0);

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