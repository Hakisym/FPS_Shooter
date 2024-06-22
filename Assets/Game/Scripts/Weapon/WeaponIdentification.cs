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

    public void Initialize(Inventory inventory)
    {
        foreach (var motion in motions)
        {
            motion.SetUp(this, inventory);
        }
    }
}
