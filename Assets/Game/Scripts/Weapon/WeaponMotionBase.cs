using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMotionBase : MonoBehaviour
{
    protected WeaponIdentification wid;
    protected WeaponController wc;
    protected Inventory inventory;
    public Transform target;

    public void SetUp(WeaponIdentification wid,Inventory inventory)
    {
        this.wid = wid;
        this.inventory = inventory;
        this.wc = inventory.wc;
    }
}
