using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraRecoilProcedural : CameraRecoil
{
    public Vector3 currentRotation;

    public Vector3 lastRotation;
    public bool canReturn;
    public bool fire;

    public float recoilKickDuration;
    public float recoilReturnDuration;
    public float timeElapsed = 0f;
    public float timeElapsedReturn = 0f;

    private Vector3 rot;

    private void Start()
    {
        wc.OnFire += () => fire = true;
    }

    private void Update()
    {
        if (!wc.preset) return;

        if (fire)
        {
            timeElapsed += Time.deltaTime;
            timeElapsed = Mathf.Clamp(timeElapsed, 0, recoilKickDuration);
            float t = timeElapsed / recoilKickDuration;

            rot = Vector3.Lerp(lastRotation, currentRotation, t);

            if (t == 1)
            {
                fire = wc.IsShooting ? true : false;
                lastRotation = currentRotation;
                timeElapsed = 0;
                timeElapsedReturn = recoilReturnDuration;
            }
        }

        //if (!fire)
        //{
        //    timeElapsedReturn -= Time.deltaTime;
        //    timeElapsedReturn = Mathf.Clamp(timeElapsedReturn, 0, recoilReturnDuration);
        //    float t = timeElapsedReturn / recoilReturnDuration;

        //    rot = Vector3.Lerp(Vector3.zero, currentRotation, t);

        //    if (t == 0)
        //    {
        //        lastRotation = Vector3.zero;
        //        currentRotation = Vector3.zero;
        //        timeElapsed = 0;
        //    }
        //}

        resultCameraRotation = rot;
    }

    //Recoil while using procedural pattern
    public override void ApplyRecoil()
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
}