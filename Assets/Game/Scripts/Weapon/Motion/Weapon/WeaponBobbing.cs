using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : WeaponMotionBase
{
    public float effectIntensity = 0.01f;
    public float effectSpeed = 10f;
    public float smoothTime = 0.1f;

    [Space]
    public float sprintBobingSpeed = 1.5f;
    public float sprintIntensity = 0.03f;
    public float aimBobbingSpeed = 0.8f;
    public float aimIntensity = 0.2f;

    private PlayerMovement pm;

    private Vector3 originalPosition;
    private float sinTime;
    private Vector3 velocity = Vector3.zero;
    private float bobbingSpeed;
    private float bobbingIntensity;

    private void Start()
    {
        pm = wc.GetComponent<PlayerMovement>();
        originalPosition = target.localPosition;
    }

    private void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        bobbingIntensity = effectIntensity;
        // Change bobbing speed with aiming & moving
        if (pm.isSprinting)
        {
            bobbingIntensity = effectIntensity + sprintIntensity;
            bobbingSpeed = effectSpeed * sprintBobingSpeed;
        }
        else if (wc.aiming || pm.isCrouching)
        {
            bobbingSpeed = effectSpeed * aimBobbingSpeed;
        }
        else
        {
            bobbingSpeed = effectSpeed;
        }
        sinTime += Time.deltaTime * bobbingSpeed;

        // Movement & air multiplier for return animation
        float idleMultiplier = inputVector.sqrMagnitude > 0f && pm.isGrounded ? 1f : 0f;

        // Calculate bobbing position with a Mobius pattern
        float sinAmountY = Mathf.Sin(sinTime) * bobbingIntensity * (wc.aiming ? aimIntensity : 1f) * idleMultiplier;
        float sinAmountX = Mathf.Sin(sinTime/2) * bobbingIntensity * (wc.aiming ? aimIntensity : 1f) * idleMultiplier;

        Vector3 finalPosition = originalPosition + Vector3.up * sinAmountY + Vector3.right * sinAmountX;

        // Smooth towards final position
        target.localPosition = Vector3.SmoothDamp(target.localPosition, finalPosition, ref velocity, smoothTime);
    }
}
