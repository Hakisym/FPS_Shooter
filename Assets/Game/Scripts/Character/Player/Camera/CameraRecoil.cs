using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : CameraMotionBase
{
    public Vector3 recoilAmount;
    public float recoilDampTime;

    public override Vector3 targetRotation { get => targetRotation = resultCameraRotation + resultRecoilRotation; }

    private WeaponController wc;
    private PlayerLook pl;
    private Vector3 currentRotation;
    private Vector3 currentRecoil;
    private Vector3 resultCameraRotation;
    private Vector3 resultRecoilRotation;
    private float evaluationProgress, evaluationProgressX;

    private void Awake()
    {
        wc = GetComponentInParent<WeaponController>();
        pl = GetComponentInParent<PlayerLook>();
    }

    private void Update()
    {
        if (!wc.preset) return;

        if (!wc.shooting || !wc.canShoot)
        {
            evaluationProgress = 0;
            evaluationProgressX = 0;
            //Return interpolation
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, wc.preset.cameraRecoilParameters.returnSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (!wc.preset) return;

        //Smooth rotate to current rotation
        resultCameraRotation = Vector3.Slerp(resultCameraRotation, currentRotation, wc.preset.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);

        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, 35 * Time.deltaTime);
        resultRecoilRotation = Vector3.Slerp(resultRecoilRotation, currentRecoil, recoilDampTime * Time.fixedDeltaTime);
    }

    public void ApplyRecoil(float vertical, float horizontal, float shakeMultiplier, bool isAiming = false)
    {
        float multiplier = isAiming ? 1f : 0.7f;

        //Recoil while using procedural pattern
        if (wc.preset.cameraRecoilParameters.type == RecoilType.Procedural)
        {
            //Add random offset to current rotation when firing
            if (wc.aiming)
            {
                currentRotation += new Vector3(-wc.preset.cameraRecoilParameters.recoilRotationAiming.x,
                    Random.Range(-wc.preset.cameraRecoilParameters.recoilRotationAiming.y, wc.preset.cameraRecoilParameters.recoilRotationAiming.y),
                    Random.Range(-wc.preset.cameraRecoilParameters.recoilRotationAiming.z, wc.preset.cameraRecoilParameters.recoilRotationAiming.z));
            }
            else
            {
                currentRotation += new Vector3(-wc.preset.cameraRecoilParameters.recoilRotation.x,
                    Random.Range(-wc.preset.cameraRecoilParameters.recoilRotation.y, wc.preset.cameraRecoilParameters.recoilRotation.y),
                    Random.Range(-wc.preset.cameraRecoilParameters.recoilRotation.z, wc.preset.cameraRecoilParameters.recoilRotation.z));
            }
        }

        //Recoil while using animation curve
        else if (wc.preset.cameraRecoilParameters.type == RecoilType.Curve)
        {
            evaluationProgress += 1f / wc.preset.gunStats.magazineSize;
            evaluationProgressX += 1f / wc.preset.gunStats.magazineSize;

            var xRecoil = wc.aiming ? wc.preset.cameraRecoilParameters.xRecoilAmount * 2 : wc.preset.cameraRecoilParameters.xRecoilAmount;
            var yRecoil = wc.aiming ? wc.preset.cameraRecoilParameters.yRecoilAmount * 2 : wc.preset.cameraRecoilParameters.yRecoilAmount;
            currentRotation = new Vector3(-wc.preset.cameraRecoilParameters.recoilY.Evaluate(evaluationProgress) * yRecoil, -wc.preset.cameraRecoilParameters.recoilX.Evaluate(evaluationProgress) * xRecoil, 0);
        }

        //Recoil while using look value
        else if (wc.preset.cameraRecoilParameters.type == RecoilType.LookValue)
        {
            pl.AddLookValue(new Vector2(vertical, Random.Range(-horizontal, horizontal)) * multiplier);
        }

        //Recoil shake
        currentRecoil += new Vector3(recoilAmount.x, Random.Range(-recoilAmount.y, recoilAmount.y), Random.Range(-recoilAmount.z, recoilAmount.z)) * multiplier * shakeMultiplier;
    }
}

public enum RecoilType
{
    Procedural,
    Curve,
    LookValue
}