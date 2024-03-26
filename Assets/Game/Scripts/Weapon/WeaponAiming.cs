using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    public WeaponController wc;
    public Inventory inventory;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 velocity;
    private bool soundHasPlayed;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
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
            finalPosition = inventory.currentWeapon.wd.aimParameters.aimPosition;
            finalRotation = Quaternion.Euler(inventory.currentWeapon.wd.aimParameters.aimRotation);
            wc.aiming = true;

            //play aim sound
            if (!soundHasPlayed)
            {
                wc.mainAudioSource.clip = wc.wd.soundClips.aimSound;
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
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref velocity, inventory.currentWeapon.wd.aimParameters.smoothTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, Time.deltaTime * inventory.currentWeapon.wd.aimParameters.smoothRotation);

    }
}
