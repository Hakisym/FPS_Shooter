using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilCurve : CameraRecoil
{
    public float evaluationProgress, evaluationProgressX;

    private void Update()
    {
        if (!currentPreset) return;
        if (wc.shootState) return;

        if (evaluationProgress > 0)
        {
            evaluationProgress -= currentPreset.cameraRecoilParameters.returnSpeed * Time.deltaTime;
        }
        else
        {
            evaluationProgress = 0;
        }

        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, currentPreset.cameraRecoilParameters.returnSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!currentPreset) return;
        //Smooth rotate to current rotation
        resultCameraRotation = Vector3.Slerp(resultCameraRotation, currentRotation, currentPreset.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);
    }

    //Recoil while using animation curve
    public override void ApplyRecoil(bool aiming)
    {
        evaluationProgress += 1f / wc.preset.gunStats.magazineSize;

        var xRecoil = currentPreset.cameraRecoilParameters.xRecoilAmount;
        var yRecoil = currentPreset.cameraRecoilParameters.yRecoilAmount;
        currentRotation = new Vector3(-currentPreset.cameraRecoilParameters.recoilY.Evaluate(evaluationProgress) * yRecoil, -currentPreset.cameraRecoilParameters.recoilX.Evaluate(evaluationProgress) * xRecoil, 0);
    }
}
