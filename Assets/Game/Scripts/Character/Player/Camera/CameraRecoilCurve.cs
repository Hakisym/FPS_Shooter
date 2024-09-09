using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoilCurve : CameraRecoil
{
    public float evaluationProgress, evaluationProgressX;

    private void Update()
    {
        if (!wc.preset) return;
        if (wc.shootState) return;

        if (evaluationProgress > 0)
        {
            evaluationProgress -= wc.preset.cameraRecoilParameters.returnSpeed * Time.deltaTime;
        }
        else
        {
            evaluationProgress = 0;
        }

        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, wc.preset.cameraRecoilParameters.returnSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!wc.preset) return;
        //Smooth rotate to current rotation
        resultCameraRotation = Vector3.Slerp(resultCameraRotation, currentRotation, wc.preset.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);
    }

    //Recoil while using animation curve
    public override void ApplyRecoil()
    {
        evaluationProgress += 1f / wc.preset.gunStats.magazineSize;

        var xRecoil = wc.preset.cameraRecoilParameters.xRecoilAmount;
        var yRecoil = wc.preset.cameraRecoilParameters.yRecoilAmount;
        currentRotation = new Vector3(-wc.preset.cameraRecoilParameters.recoilY.Evaluate(evaluationProgress) * yRecoil, -wc.preset.cameraRecoilParameters.recoilX.Evaluate(evaluationProgress) * xRecoil, 0);
    }
}
