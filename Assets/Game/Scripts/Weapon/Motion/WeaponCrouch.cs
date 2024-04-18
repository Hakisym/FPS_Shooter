using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCrouch : WeaponMotionBase
{
    public Vector3 crouchPosition;
    public Vector3 crouchRotation;
    public float crouchSpeed;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private PlayerMovement pm;
    private float multiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        pm = wc.GetComponent<PlayerMovement>();

        originalPosition = target.localPosition;
        originalRotation = target.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 finalPosition;
        Quaternion finalRotation;

        //Update transform to aim position & rotation
        if (pm.isCrouching && !wc.aiming)
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
