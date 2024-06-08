using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : WeaponMotionBase
{
    [Header("PositionSway")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("RotationSway")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;

    [Space]
    public bool rotationX = true;
    public bool rotationY = true;
    public bool rotationZ = true;
    public float aimMultiplier = 0.2f;
    private float currentAimMultiplier;
    private bool controllable;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float inputX;
    private float inputY;

    private void Start()
    {
        initialPosition = target.localPosition;
        initialRotation = target.localRotation;

        controllable = true;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.controllable = !paused;
    }

    private void Update()
    {
        currentAimMultiplier = wc.aiming ? aimMultiplier : 1f;

        CheckInput();
        UpdatePositionsway();
        UpdateRotationSway();
    }

    private void CheckInput()
    {
        if (!controllable) return;
        // Controls
        inputX = Input.GetAxis("Mouse X");
        inputY = Input.GetAxis("Mouse Y");
    }

    private void UpdatePositionsway()
    {
        // Smooth position sway
        float moveX = Mathf.Clamp(inputX * amount * currentAimMultiplier, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(inputY * amount * currentAimMultiplier, -maxAmount, maxAmount);

        Vector3 finalPosition = initialPosition + new Vector3(moveX, moveY, 0);

        target.localPosition = Vector3.Lerp(target.localPosition, finalPosition, smoothAmount * Time.deltaTime);
    }

    private void UpdateRotationSway()
    {
        float tiltX = Mathf.Clamp(inputX * rotationAmount * currentAimMultiplier, -maxRotationAmount, maxRotationAmount);
        float tiltY = Mathf.Clamp(inputY * rotationAmount * currentAimMultiplier, -maxRotationAmount, maxRotationAmount);

        // Calculate target rotation
        Quaternion rotX = rotationX ? Quaternion.AngleAxis(tiltY, Vector3.right) : Quaternion.identity;
        Quaternion rotY = rotationY ? Quaternion.AngleAxis(-tiltX, Vector3.up) : Quaternion.identity;
        Quaternion rotZ = rotationZ ? Quaternion.AngleAxis(-tiltX, Vector3.forward) : Quaternion.identity;

        Quaternion finalRotation = initialRotation * rotX * rotY * rotZ;

        // Rotate towards target rotation
        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, smoothRotation * Time.deltaTime);
    }
}
