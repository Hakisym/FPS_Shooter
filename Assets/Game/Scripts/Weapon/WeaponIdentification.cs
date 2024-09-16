using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIdentification : MonoBehaviour
{
    public WeaponData preset;
    public Transform muzzle;
    public Transform castPoint;
    public Animator animator;
    public WeaponMotionBase[] motions;

    public bool aimState;
    public float aimValue;

    public void Initialize(WeaponController wc, PlayerMovement pm)
    {
        wc.OnAimStateChange += (bool isAiming) => aimState = isAiming;
        wc.OnAimValueUpdate += (float t) => aimValue = t;

        foreach (var motion in motions)
        {
            motion.Setup(this, pm);
        }
    }
}
