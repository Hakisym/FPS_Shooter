using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HenryUtilities
{
    public static Vector3 GetSpreadDirection(float amount, Camera camera)
    {
        float horSpread = Random.Range(-amount, amount);
        float verSpread = Random.Range(-amount, amount);
        Vector3 spread = camera.transform.InverseTransformDirection(new Vector3(horSpread, verSpread, 0));

        return spread;
    }

    public static Vector3 MultiplyVectors(Vector3 a, Vector3 b)
    {
        a.x *= b.x;
        a.y *= b.y;
        a.z *= b.z;

        return a;
    }
}
