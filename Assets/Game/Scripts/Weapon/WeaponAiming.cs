using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAiming : WeaponMotionBase
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 velocity;
    private bool soundHasPlayed;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = target.localPosition;
        originalRotation = target.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = Input.GetMouseButton(1);
        Vector3 finalPosition;
        Quaternion finalRotation;

        //Update transform to aim position & rotation
        if (isAiming)
        {
            finalPosition = wid.wd.aimParameters.aimPosition;
            finalRotation = Quaternion.Euler(wid.wd.aimParameters.aimRotation);
            wc.aiming = true;

            //play aim sound
            if (!soundHasPlayed)
            {
                wc.mainAudioSource.clip = wid.wd.soundClips.aimSound;
                wc.mainAudioSource.Play();
                soundHasPlayed = true;
            }
        }
        else
        {
            finalPosition = originalPosition;
            finalRotation = originalRotation;
            wc.aiming = false;
            //reset aim sound
            soundHasPlayed = false;
        }

        //Smooth interpolate
        target.localPosition = Vector3.SmoothDamp(target.localPosition, finalPosition, ref velocity, wid.wd.aimParameters.smoothTime);
        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, Time.deltaTime * wid.wd.aimParameters.smoothRotation);

    }
}
