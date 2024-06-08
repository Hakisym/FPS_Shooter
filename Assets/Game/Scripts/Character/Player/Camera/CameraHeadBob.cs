using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHeadBob : CameraMotionBase
{
    public float headBobAmplitude;
    public float headBobFrequency;

    private PlayerMovement pm;

    private void Awake()
    {
        pm = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        float angle = Time.timeSinceLevelLoad * headBobFrequency;
        float distanceY = headBobAmplitude * Mathf.Sin(angle) / 400f;
        float distanceX = headBobAmplitude * Mathf.Cos(angle) / 100f;

        if (pm.moveVel < 0.2f || !pm.isGrounded)
        {
            targetPosition = Vector3.Lerp(targetPosition, Vector3.zero, Time.deltaTime * 2f);
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * 2f);
        }
        else
        {
            targetPosition = new Vector3(targetPosition.x, targetPosition.y + Vector3.up.y * distanceY, targetPosition.z);
            targetRotation = new Vector3(targetRotation.x + Vector3.right.x * distanceX, targetRotation.y, targetRotation.z);
        }
    }
}
