using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickable : MonoBehaviour
{
    public WeaponData weaponData;

    private void Start()
    {
        var weapon = Instantiate(weaponData.pickUpGraphics, transform);
        weapon.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    public void Interact()
    {
        if (Inventory.instance.SwapWeapon(weaponData))
            Destroy(gameObject);
    }
}
