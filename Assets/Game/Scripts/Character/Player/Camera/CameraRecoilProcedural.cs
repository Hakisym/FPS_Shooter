using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRecoilProcedural : CameraRecoil
{
    public Coroutine lerpCoroutine;
    public Vector3 endValue;
    public Vector3 lastRotation;
    [Space]
    public float startXInput;
    public float endXInput;
    [Space]
    public float startCamRot;
    public float endCamRot;

    public bool returnZero;

    public override void Awake()
    {
        base.Awake();
        returnZero = true;
    }

    private void Start()
    {
        wc.OnStartShoot += () =>
        {
            //set a return value from original fire point
            if (returnZero)
            {
                startXInput = pl.xRotation;
                startCamRot = pl.cameraRotationX;

                endValue = lastRotation;
                returnZero = false;
            }
        };

        wc.OnEndShoot += () =>
        {
            endXInput = pl.xRotation;
            endCamRot = pl.cameraRotationX;

            //return last fire point if the camera is lower than original fire point
            if (endCamRot > startCamRot)
            {
                endValue = lastRotation;
                lerpCoroutine = StartCoroutine(RecoilReturn(currentRotation, endValue));
                return;
            }

            //compensate the mouse input
            float inputAmountX = endXInput - startXInput;
            //don't compensate for over pitch input
            inputAmountX = inputAmountX < 0 ? 0f : inputAmountX;

            lerpCoroutine = StartCoroutine(RecoilReturn(currentRotation, endValue - inputAmountX * Vector3.right));

        };
    }

    IEnumerator RecoilReturn(Vector3 startValue,Vector3 endValue)
    {
        float timeElapsed = 0;
        float duration = wc.preset.cameraRecoilParameters.returnDuration;

        while (timeElapsed < duration)
        {
            currentRotation = Vector3.Lerp(startValue, endValue, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        //reset fire point
        currentRotation = endValue;
        returnZero = true;
    }

    private void Update()
    {
        if (!wc.preset) return;
        //Smooth rotate to current rotation
        resultCameraRotation = Vector3.Slerp(resultCameraRotation, currentRotation, wc.preset.cameraRecoilParameters.rotationSpeed * Time.deltaTime);
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