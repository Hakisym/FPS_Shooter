using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HenryUtilities
{
    public static Vector3 GetDirectionToCenterScreen()
    {
        Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        Ray ray = Camera.main.ScreenPointToRay(centerScreen);
        return ray.direction.normalized;
    }

    public static Vector3 GetSpreadDirection(float amount, Camera camera)
    {
        float horSpread = Random.Range(-amount, amount);
        float verSpread = Random.Range(-amount, amount);
        Vector3 spread = camera.transform.InverseTransformDirection(new Vector3(horSpread, verSpread, 0));

        return spread;
    }
}
