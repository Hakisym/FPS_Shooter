using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSprinting : WeaponMotionBase
{
    public Vector3 sprintPosition;
    public Vector3 sprintRotation;
    public float sprintSpeed;

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
        if (pm.isSprinting)
        {
            finalPosition = sprintPosition;
            finalRotation = Quaternion.Euler(sprintRotation);
        }
        else
        {
            finalPosition = originalPosition;
            finalRotation = originalRotation;
        }

        //Smooth interpolate
        target.localPosition = Vector3.Slerp(target.localPosition, finalPosition, Time.deltaTime * sprintSpeed * multiplier);
        target.localRotation = Quaternion.Slerp(target.localRotation, finalRotation, Time.deltaTime * sprintSpeed * multiplier);
    }
}
