using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRecoilProcedural : CameraRecoil
{
    private Coroutine lerpCoroutine;
    private Vector3 endValue;
    private Vector3 lastRotation;

    private float startXInput;
    private float endXInput;

    private float startCamRot;
    private float endCamRot;

    public float cameraRecoilX;
    public float cameraRecoilY;
    private Vector3 lastCameraRotation;

    public bool returnZero;

    public override void Awake()
    {
        base.Awake();
        returnZero = true;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        wc.OnStartShoot += OnStartShoot;
        wc.OnEndShoot += OnEndShoot;
    }

    private void OnDisable()
    {
        wc.OnStartShoot -= OnStartShoot;
        wc.OnEndShoot -= OnEndShoot;
    }

    void OnStartShoot()
    {
        //set a return value from original fire point
        if (returnZero)
        {
            startXInput = pl.xInput;
            startCamRot = pl.xRotation;

            endValue = lastRotation;
            returnZero = false;
        }
    }

    void OnEndShoot()
    {
        endXInput = pl.xInput;
        endCamRot = pl.xRotation;

        //return last fire point if the camera is lower than original fire point
        if (endCamRot > startCamRot)
        {
            endValue = lastRotation;
            lerpCoroutine = StartCoroutine(RecoilReturn(currentRotation, endValue, currentPreset.cameraRecoilParameters.returnDuration));
            return;
        }

        //compensate the mouse input
        float inputAmountX = endXInput - startXInput;
        //don't compensate for over pitch input
        inputAmountX = inputAmountX < 0 ? 0f : inputAmountX;

        lerpCoroutine = StartCoroutine(RecoilReturn(currentRotation, endValue - inputAmountX * Vector3.right, currentPreset.cameraRecoilParameters.returnDuration));

    }

    IEnumerator RecoilReturn(Vector3 startValue,Vector3 endValue, float duration)
    {
        float timeElapsed = 0;

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
        if (!currentPreset) return;
        //Smooth rotate to current rotation
        resultCameraRotation = Vector3.Slerp(resultCameraRotation, currentRotation, currentPreset.cameraRecoilParameters.rotationSpeed * Time.deltaTime);

        cameraRecoilX = resultCameraRotation.x - lastCameraRotation.x;
        cameraRecoilY = resultCameraRotation.y - lastCameraRotation.y;
        lastCameraRotation = resultCameraRotation;
    }

    //Recoil while using procedural pattern
    public override void ApplyRecoil(bool aiming)
    {
        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        lastRotation = currentRotation;

        //Add random offset to current rotation when firing
        if (aiming)
        {
            currentRotation += new Vector3(-currentPreset.cameraRecoilParameters.recoilRotationAiming.x,
                Random.Range(-currentPreset.cameraRecoilParameters.recoilRotationAiming.y, currentPreset.cameraRecoilParameters.recoilRotationAiming.y),
                Random.Range(-currentPreset.cameraRecoilParameters.recoilRotationAiming.z, currentPreset.cameraRecoilParameters.recoilRotationAiming.z));
        }
        else
        {
            currentRotation += new Vector3(-currentPreset.cameraRecoilParameters.recoilRotation.x,
                Random.Range(-currentPreset.cameraRecoilParameters.recoilRotation.y, currentPreset.cameraRecoilParameters.recoilRotation.y),
                Random.Range(-currentPreset.cameraRecoilParameters.recoilRotation.z, currentPreset.cameraRecoilParameters.recoilRotation.z));
        }
    }
}