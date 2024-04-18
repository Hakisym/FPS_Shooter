using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponJumping : WeaponMotionBase
{
    public float effectIntensity = 16f;
    public float effectIntensityAiming = 8f;
    public float smoothTime = 12f;
    public float animationSpeed = 18f;
    [Space]
    private PlayerMovement pm;

    private Quaternion originalRotation;
    private float shakeAmount;
    private bool wasGrounded;

    private void Start()
    {
        pm = wc.GetComponent<PlayerMovement>();
        originalRotation = target.localRotation;
        wasGrounded = pm.isGrounded;
    }

    private void Update()
    {
        // Check if the player has just landed on the ground
        if (!wasGrounded && pm.isGrounded)
        {
            // Set the jump intensity
            shakeAmount = wc.aiming ? effectIntensityAiming : effectIntensity;
        }

        // Gradually decrease jumpAmount over time for a smoother animation
        shakeAmount = Mathf.Lerp(shakeAmount, 0f, Time.deltaTime * animationSpeed);

        Quaternion finalRotation = originalRotation * Quaternion.Euler(Vector3.right * shakeAmount);

        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, smoothTime * Time.deltaTime);

        wasGrounded = pm.isGrounded; // Update the previous grounded state for the next frame
    }
}
