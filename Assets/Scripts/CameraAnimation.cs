using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimation : MonoBehaviour
{
    public float effectIntensity = 6f;
    public float effectIntensityAiming = 3f;
    public float smoothTime = 12f;
    public float animationSpeed = 12f;
    [Space]
    public PlayerMovement pm;
    public PlayerBehaviour pb;

    private Quaternion originalRotation;
    private float shakeAmount;
    private bool wasGrounded;

    private void Start()
    {
        originalRotation = transform.localRotation;
        wasGrounded = pm.isGrounded;
    }

    private void Update()
    {
        // Check if the player has just landed on the ground
        if (!wasGrounded && pm.isGrounded)
        {
            // Set the jump intensity
            shakeAmount = pb.isAiming ? effectIntensityAiming : effectIntensity;
        }

        // Gradually decrease jumpAmount over time for a smoother animation
        shakeAmount = Mathf.Lerp(shakeAmount, 0f, Time.deltaTime * animationSpeed);

        Quaternion finalRotation = originalRotation * Quaternion.Euler(Vector3.right * shakeAmount);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, smoothTime * Time.deltaTime);

        wasGrounded = pm.isGrounded; // Update the previous grounded state for the next frame
    }
}
