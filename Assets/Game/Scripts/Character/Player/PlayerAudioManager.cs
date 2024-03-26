using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    public AudioSource mainAudioSource;
    private WeaponController wc;

    private bool soundHasPlayed;

    private void Start()
    {
        wc = GetComponent<WeaponController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wc.aiming)
        {
            if (!soundHasPlayed)
            {
                mainAudioSource.clip = wc.wd.soundClips.aimSound;
                mainAudioSource.Play();
                soundHasPlayed = true;
            }
        }
        else
            soundHasPlayed = false;
    }
}
