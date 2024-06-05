using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLandMotion : CameraMotionBase
{
    public float effectIntensity = 6f;
    public float smoothTime = 12f;
    public float animationSpeed = 12f;

    private float shakeAmount;
    private Vector3 finalRotation;

    private void Start()
    {
        GetComponentInParent<PlayerMovement>().OnLand.AddListener(Trigger);
    }

    private void Update()
    {
        // Gradually decrease jumpAmount over time for a smoother animation
        shakeAmount = Mathf.Lerp(shakeAmount, 0f, Time.deltaTime * animationSpeed);

        finalRotation = Vector3.Slerp(finalRotation, Vector3.right * shakeAmount, Time.deltaTime * smoothTime);

        targetRotation = finalRotation;
    }

    public void Trigger()
    {
        // Set the jump intensity
        shakeAmount = effectIntensity;
    }
}
