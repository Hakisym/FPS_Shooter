using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAiming : WeaponMotionBase
{
    public Vector3 aimPosition;
    private float aimDuration;
    private float timeElapsed = 0f;

    private Vector3 originalPosition;
    private bool soundHasPlayed;
    private float lastDuration;
    private bool wasAiming;

    private CameraFovManager cameraFovManager;

    private void Awake()
    {
        cameraFovManager = GetComponentInParent<CameraManager>().cameraFovManager;
        originalPosition = target.localPosition;
    }

    private void Start()
    {
        aimDuration = wc.preset.gunStats.adsTime;
    }

    void Update()
    {
        //Update transform to aim position & rotation
        if (wc.aiming)
        {
            //play aim sound
            if (!soundHasPlayed)
            {
                //wc.mainAudioSource.clip = wid.wd.soundClips.aimSound;
                //wc.mainAudioSource.Play();
                soundHasPlayed = true;
            }

            timeElapsed += Time.deltaTime;

            if (!wasAiming) wc.OnAimStart?.Invoke();
            wasAiming = true;
        }
        else
        {
            //reset aim sound
            soundHasPlayed = false;

            timeElapsed -= Time.deltaTime;

            if (wasAiming) wc.OnAimCancel?.Invoke();
            wasAiming = false;
        }

        //clamp ads time
        timeElapsed = Mathf.Clamp(timeElapsed, 0, aimDuration);
        //ads gradually over time
        float t = timeElapsed / aimDuration;

        if (t == 1 && lastDuration < 1) wc.OnAimFinish?.Invoke();
        lastDuration = t;

        target.localPosition = Vector3.Lerp(originalPosition, aimPosition, t);

        cameraFovManager.UpdateAimFov(t);
        wc.UpdateAimSpread(t);
    }
}
