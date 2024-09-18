using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAiming : WeaponMotionBase
{
    public Vector3 aimPosition;

    void Update()
    {
        target.localPosition = Vector3.Lerp(originalPosition, aimPosition, stateProvider.AimValue);
    }
}
