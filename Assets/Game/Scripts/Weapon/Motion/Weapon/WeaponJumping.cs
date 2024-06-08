using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponJumping : WeaponMotionBase
{
    public Vector3 position = new Vector3(0, -0.2f, 0);
    public Vector3 rotation = new Vector3(5, 0, 0);
    public float returnSpeed = 35f;
    public float smoothTime = 10f;

    [Space]
    private PlayerMovement pm;

    private Quaternion originalRotation;
    private Vector3 currentPosition;
    private Vector3 currentRotation;
    private Vector3 finalPosition;
    private Vector3 finalRotation;

    private void Start()
    {
        pm = wc.GetComponent<PlayerMovement>();
        pm.OnLand.AddListener(Trigger);

        originalRotation = target.localRotation;
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

    public void Trigger()
    {
        float multiplier = wc.aiming ? 1 : 0.7f;

        // Set the jump intensity
        currentPosition += position * multiplier;
        currentRotation += rotation * multiplier;
    }
}
