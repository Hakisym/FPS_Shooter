using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    public Camera Camera { get; private set; }
    public AudioListener AudioListener { get; private set; }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        AudioListener = GetComponent<AudioListener>();

        Disable();
    }

    public void Enable()
    {
        Camera.enabled = true;
        AudioListener.enabled = true;
    }

    public void Disable()
    {
        Camera.enabled = false;
        AudioListener.enabled = false;
    }
}
