using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCrouch : WeaponMotionBase
{
    public Vector3 crouchPosition;
    public Vector3 crouchRotation;
    public float crouchSpeed;

    private float multiplier = 1f;


    // Update is called once per frame
    void Update()
    {
        Vector3 finalPosition;
        Quaternion finalRotation;

        //Update transform to aim position & rotation
        if (stateProvider.CurrentMoveState == MoveState.Crouching && !stateProvider.IsAiming)
        {
            finalPosition = crouchPosition;
            finalRotation = Quaternion.Euler(crouchRotation);
        }
        else
        {
            finalPosition = originalPosition;
            finalRotation = originalRotation;
        }

        //Smooth interpolate
        target.localPosition = Vector3.Lerp(target.localPosition, finalPosition, Time.deltaTime * crouchSpeed * multiplier);
        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, Time.deltaTime * crouchSpeed * multiplier);
    }
}
