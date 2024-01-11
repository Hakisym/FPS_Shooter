using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{
    public float effectIntensity = 0.02f;
    public float effectSpeed = 18f;
    public float smoothTime = 0.1f;
    public PlayerBehaviour pb;
    public PlayerMovement pm;

    private Vector3 originalPosition;
    private float sinTime;
    private Vector3 velocity = Vector3.zero;
    private float bobbingSpeed;
    private float bobbingIntensity;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        bobbingIntensity = effectIntensity;
        // Change bobbing speed with aiming & moving
        if (pm.isSprinting)
        {
            bobbingIntensity = effectIntensity + 0.01f;
        }
        else if (pm.isCrouching || pb.isAiming)
        {
            bobbingSpeed = effectSpeed * 0.5f;
        }
        else
        {
            bobbingSpeed = effectSpeed;
        }
        sinTime += Time.deltaTime * bobbingSpeed;

        // Movement multiplier for return animation
        float idleMultiplier = inputVector.sqrMagnitude > 0f ? 1f : 0f;

        // Calculate bobbing position with a Mobius pattern
        float sinAmountY = Mathf.Sin(sinTime) * bobbingIntensity * pb.currentAimBobbingMultiplier * idleMultiplier;
        float sinAmountX = Mathf.Sin(sinTime/2) * bobbingIntensity * pb.currentAimBobbingMultiplier * idleMultiplier;

        Vector3 finalPosition = originalPosition + Vector3.up * sinAmountY + Vector3.right * sinAmountX;

        // Smooth towards final position
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref velocity, smoothTime);
    }
}
