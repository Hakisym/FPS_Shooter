using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRecoilProcedural : CameraRecoil
{
    public float lerpDuration;
    public Coroutine lerpCoroutine;
    public Vector3 endValue;
    private Vector3 lastRotation;

    private float startXInput;
    private float endXInput;

    private float startCamRot;
    private float endCamRot;


    private void Start()
    {
        wc.OnStartShoot += () =>
        {
            startXInput = pl.xRotation;
            startCamRot = pl.cameraRotationX;
            endValue = lastRotation;
        };

        wc.OnEndShoot += () =>
        {
            endXInput = pl.xRotation;
            endCamRot = pl.cameraRotationX;

            //dont return if the camera is lower than original fire point
            if (endCamRot > startCamRot) return;

            //return to the original fire point, compensate the mouse input
            float inputAmountX = endXInput - startXInput;
            lerpCoroutine = StartCoroutine(RecoilReturn(currentRotation, endValue - inputAmountX * Vector3.right));
        };
    }

    IEnumerator RecoilReturn(Vector3 startValue,Vector3 endValue)
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            currentRotation = Vector3.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        currentRotation = endValue;
    }

    private void FixedUpdate()
    {
        if (!wc.preset) return;
        //Smooth rotate to current rotation
        resultCameraRotation = Vector3.Slerp(resultCameraRotation, currentRotation, wc.preset.cameraRecoilParameters.rotationSpeed * Time.fixedDeltaTime);
    }

    //Recoil while using procedural pattern
    public override void ApplyRecoil()
    {
        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        lastRotation = currentRotation;

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
}