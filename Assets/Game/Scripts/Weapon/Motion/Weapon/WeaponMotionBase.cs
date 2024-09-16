using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMotionBase : MonoBehaviour
{
    protected WeaponIdentification wid;
    protected PlayerMovement pm;
    public Transform target;

    public void Setup(WeaponIdentification wid, PlayerMovement pm)
    {
        this.wid = wid;
        this.pm = pm;
    }
}
