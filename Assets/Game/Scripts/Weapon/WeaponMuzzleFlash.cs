using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMuzzleFlash : MonoBehaviour
{
    public void AttachMuzzleFlash(Transform muzzle)
    {
        transform.SetParent(muzzle);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void DetachMuzzleFlash(Transform holder)
    {
        transform.SetParent(holder);
    }
}
