using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
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
    public PlayerBehaviour pb;


    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float inputX;
    private float inputY;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        pb = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
    }

    private void Update()
    {
        CheckInput();
        UpdatePositionsway();
        UpdateRotationSway();
    }

    private void CheckInput()
    {
        // Controls
        inputX = Input.GetAxis("Mouse X");
        inputY = Input.GetAxis("Mouse Y");
    }

    private void UpdatePositionsway()
    {
        // Smooth position sway
        float moveX = Mathf.Clamp(inputX * amount * pb.currentAimSwayMultiplier, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(inputY * amount * pb.currentAimSwayMultiplier, -maxAmount, maxAmount);

        Vector3 finalPosition = initialPosition + new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, smoothAmount * Time.deltaTime);
    }

    private void UpdateRotationSway()
    {
        float tiltX = Mathf.Clamp(inputX * rotationAmount * pb.currentAimSwayMultiplier, -maxRotationAmount, maxRotationAmount);
        float tiltY = Mathf.Clamp(inputY * rotationAmount * pb.currentAimSwayMultiplier, -maxRotationAmount, maxRotationAmount);

        // Calculate target rotation
        Quaternion rotX = rotationX ? Quaternion.AngleAxis(tiltY, Vector3.right) : Quaternion.identity;
        Quaternion rotY = rotationY ? Quaternion.AngleAxis(-tiltX, Vector3.up) : Quaternion.identity;
        Quaternion rotZ = rotationZ ? Quaternion.AngleAxis(-tiltX, Vector3.forward) : Quaternion.identity;

        Quaternion finalRotation = initialRotation * rotX * rotY * rotZ;

        // Rotate towards target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, smoothRotation * Time.deltaTime);
    }
}
