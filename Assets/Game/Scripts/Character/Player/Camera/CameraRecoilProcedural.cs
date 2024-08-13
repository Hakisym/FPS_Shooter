using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRecoilProcedural : CameraRecoil
{
    public Vector3 currentRotation;
    public bool canReturn;
    public float lerpDuration;
    public Coroutine lerpCoroutine;

    private void Start()
    {
        wc.OnEndShoot += () =>
            lerpCoroutine = StartCoroutine(RecoilReturn(currentRotation));
    }

    IEnumerator RecoilReturn(Vector3 startValue)
    {
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            currentRotation = Vector3.Lerp(startValue, Vector3.zero, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        currentRotation = Vector3.zero;
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