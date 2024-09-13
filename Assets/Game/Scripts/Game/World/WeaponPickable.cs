using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickable : MonoBehaviour
{
    public WeaponData weaponData;
    public bool canInteract;

    private void Start()
    {
        Instantiate(weaponData.pickUpGraphics, transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canInteract)
            if(other.GetComponentInChildren<Inventory>()!=null)
                if (other.GetComponentInChildren<Inventory>().PickupWeapon(weaponData))
                    Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        canInteract = true;
    }
}
