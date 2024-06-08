using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAiming : WeaponMotionBase
{
    public Vector3 aimPosition;
    public Vector3 aimRotation;
    public float smoothTime;
    public float smoothRotation;

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
        Vector3 finalPosition;
        Quaternion finalRotation;

        //Update transform to aim position & rotation
        if (wc.aiming)
        {
            finalPosition = aimPosition;
            finalRotation = Quaternion.Euler(aimRotation);

            //play aim sound
            if (!soundHasPlayed)
            {
                //wc.mainAudioSource.clip = wid.wd.soundClips.aimSound;
                //wc.mainAudioSource.Play();
                soundHasPlayed = true;
            }
        }
        else
        {
            finalPosition = originalPosition;
            finalRotation = originalRotation;

            //reset aim sound
            soundHasPlayed = false;
        }

        //Smooth interpolate
        target.localPosition = Vector3.SmoothDamp(target.localPosition, finalPosition, ref velocity, smoothTime);
        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, Time.deltaTime * smoothRotation);

    }
}
