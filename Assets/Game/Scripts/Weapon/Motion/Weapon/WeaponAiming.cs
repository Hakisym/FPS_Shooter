using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponAiming : WeaponMotionBase
{
    public Vector3 aimPosition;
    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = target.localPosition;
    }

    void Update()
    {
        target.localPosition = Vector3.Lerp(originalPosition, aimPosition, wid.aimValue);
    }
}
