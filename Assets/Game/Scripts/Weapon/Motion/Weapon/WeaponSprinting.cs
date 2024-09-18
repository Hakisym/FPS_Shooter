using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprinting : WeaponMotionBase
{
    public Vector3 sprintPosition;
    public Vector3 sprintRotation;
    public float sprintSpeed;

    private float multiplier = 1f;


    // Update is called once per frame
    void Update()
    {
        Vector3 finalPosition;
        Quaternion finalRotation;

        //Update transform to aim position & rotation
        if (stateProvider.CurrentMoveState == MoveState.Sprinting)
        {
            finalPosition = sprintPosition;
            finalRotation = Quaternion.Euler(sprintRotation);
        }
        else
        {
            finalPosition = originalPosition;
            finalRotation = originalRotation;
        }
        var curMpr = stateProvider.IsAiming ? multiplier * 3f : multiplier;

        //Smooth interpolate
        target.localPosition = Vector3.Slerp(target.localPosition, finalPosition, Time.deltaTime * sprintSpeed * curMpr);
        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, Time.deltaTime * sprintSpeed * curMpr);
    }
}
