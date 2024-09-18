using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAirMotion : WeaponMotionBase
{
    public Vector3 jumpPosition = new Vector3(0, -0.06f, 0);
    public Vector3 jumpRotation = new Vector3(3, 0, 0.5f);
    public Vector3 landPosition = new Vector3(0, -0.1f, 0);
    public Vector3 landRotation = new Vector3(10, 0, 0);
    public float returnSpeed = 10f;
    public float smoothTime = 12f;

    private Vector3 currentPosition;
    private Vector3 currentRotation;
    private Vector3 finalPosition;
    private Vector3 finalRotation;

    public override void Setup(IWeaponStateProvider provider)
    {
        base.Setup(provider);
        provider.OnJump += Jump;
        provider.OnLand += Land;
    }

    private void Update()
    {
        // Gradually decrease jumpAmount over time for a smoother animation
        currentPosition = Vector3.Lerp(currentPosition, Vector3.zero, Time.deltaTime * returnSpeed);
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, Time.deltaTime * returnSpeed);

        finalPosition = Vector3.Slerp(finalPosition, currentPosition, Time.deltaTime * smoothTime);
        finalRotation = Vector3.Slerp(finalRotation, currentRotation, Time.deltaTime * smoothTime);

        target.localPosition = finalPosition;
        target.localRotation = originalRotation * Quaternion.Euler(finalRotation);
    }

    private void Jump()
    {
        float multiplier = stateProvider.IsAiming ? 1 : 0.7f;

        // Set the jump intensity
        currentPosition += jumpPosition * multiplier;
        currentRotation += jumpRotation * multiplier;
    }

    private void Land()
    {
        float multiplier = stateProvider.IsAiming ? 1 : 0.7f;

        // Set the jump intensity
        currentPosition += landPosition * multiplier;
        currentRotation += landRotation * multiplier;
    }

    private void OnDestroy()
    {
        stateProvider.OnJump -= Jump;
        stateProvider.OnLand -= Land;
    }
}
