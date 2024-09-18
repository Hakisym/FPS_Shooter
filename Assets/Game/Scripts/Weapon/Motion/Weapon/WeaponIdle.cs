using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIdle : WeaponMotionBase
{
    public Vector3 amount;
    public Vector3 speed = new Vector3(1, 1, 1);

    private Vector3 result;
    private Vector3 time;

    public void Update()
    {
        float multiplier = stateProvider.IsAiming ? 0.1f : 1f;

        time.x += Time.deltaTime * speed.x;
        time.y += Time.deltaTime * speed.y;
        time.z += Time.deltaTime * speed.z;

        result.x = amount.x * MathF.Sin(time.x) * multiplier;
        result.y = amount.y * MathF.Sin(time.y) * multiplier;
        result.z = amount.z * MathF.Sin(time.z) * multiplier;

        target.localPosition = Vector3.Slerp(target.localPosition, result, Time.fixedDeltaTime * 10f);
    }
}
