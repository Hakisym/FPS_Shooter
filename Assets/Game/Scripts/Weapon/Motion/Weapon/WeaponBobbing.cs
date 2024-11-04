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
    public float sprintIntensityX = 0.03f;
    public float sprintIntensityY = 0.03f;
    public float aimBobbingSpeed = 0.8f;

    private float sinTime;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        float bobbingIntensityX = effectIntensity;
        float bobbingIntensityY = effectIntensity;
        float bobbingSpeed;

        // Change bobbing speed with aiming & moving
        if (stateProvider.CurrentMoveState == MoveState.Sprinting)
        {
            bobbingIntensityX += sprintIntensityX;
            bobbingIntensityY += sprintIntensityY;
            bobbingSpeed = effectSpeed * sprintBobingSpeed;
        }
        else if (stateProvider.IsAiming || stateProvider.CurrentMoveState == MoveState.Crouching)
        {
            bobbingSpeed = effectSpeed * aimBobbingSpeed;
        }
        else
        {
            bobbingSpeed = effectSpeed;
        }

        sinTime += Time.deltaTime * bobbingSpeed;

        // Movement & air multiplier for return animation
        float idleMultiplier = !stateProvider.isVelocityZero && stateProvider.CurrentMoveState != MoveState.InAir ? 1f : 0f;

        // Calculate bobbing position with a Mobius pattern
        float sinAmountY = Mathf.Sin(sinTime) * bobbingIntensityX * (stateProvider.IsAiming ? 0.05f : 1f) * idleMultiplier;
        float sinAmountX = Mathf.Sin(sinTime/2) * bobbingIntensityY * (stateProvider.IsAiming ? 0.05f : 1f) * idleMultiplier;

        Vector3 finalPosition = originalPosition + Vector3.up * sinAmountY + Vector3.right * sinAmountX;

        // Smooth towards final position
        target.localPosition = Vector3.SmoothDamp(target.localPosition, finalPosition, ref velocity, smoothTime);
    }
}
