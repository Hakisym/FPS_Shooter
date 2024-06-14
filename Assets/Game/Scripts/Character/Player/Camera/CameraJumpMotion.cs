using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraJumpMotion : CameraMotionBase
{
    public Vector3 jumpPosition = new Vector3(0, 0.2f, 0);
    public Vector3 jumpRotation = new Vector3(-5, 0, 0);
    public float returnSpeed = 10f;
    public float smoothTime = 10f;

    private Vector3 currentPosition;
    private Vector3 currentRotation;
    private Vector3 finalPosition;
    private Vector3 finalRotation;

    private void Start()
    {
        GetComponentInParent<PlayerMovement>().OnJump.AddListener(Jump);
    }

    private void Update()
    {
        // Gradually decrease jumpAmount over time for a smoother animation
        currentPosition = Vector3.Lerp(currentPosition, Vector3.zero, Time.deltaTime * returnSpeed);
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, Time.deltaTime * returnSpeed);

        finalPosition = Vector3.Slerp(finalPosition, currentPosition, Time.deltaTime * smoothTime);
        finalRotation = Vector3.Slerp(finalRotation, currentRotation, Time.deltaTime * smoothTime);

        targetPosition = finalPosition;
        targetRotation = finalRotation;
    }

    private void Jump()
    {
        // Set the jump intensity
        currentPosition += new Vector3(Random.Range(jumpPosition.x, -jumpPosition.x), Random.Range(jumpPosition.y, -jumpPosition.y), jumpPosition.z);
        currentRotation += new Vector3(jumpRotation.x, Random.Range(jumpRotation.y, -jumpRotation.y), Random.Range(jumpRotation.z, -jumpRotation.z));
    }
}
