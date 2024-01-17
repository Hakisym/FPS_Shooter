using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public WeaponSystem[] weapons;
    public WeaponSystem currentWeapon;
    private int weaponIndex = -1;

    public bool switching;

    public float switchDelay = 1.0f;  // Adjust the delay time as needed
    private float lastSwitchTime;

    //animation
    public Animator animator;

    private void Start()
    {
        weapons = GetComponentsInChildren<WeaponSystem>(true);

        // equip at start
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].gameObject.activeSelf && currentWeapon == null)
            {
                currentWeapon = weapons[i];
                weaponIndex = i;
            }
            else
            {
                weapons[i].gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        // Check if there's a valid scroll input
        if (scrollValue != 0f)
        {
            // Check if enough time has passed since the last switch
            if (Time.time - lastSwitchTime >= switchDelay)
            {
                int index = weaponIndex;
                // Switch weapon
                if(scrollValue>0) Equip(index + 1);
                else Equip(index - 1);

                // Update the last switch time
                lastSwitchTime = Time.time;
            }
        }
    }

    public void Equip(int index)
    {
        //state check
        if (switching) return;
        if (currentWeapon != null && (currentWeapon.reloading || currentWeapon.shooting))
        {
            Debug.Log("Can't switch when reloading and shooting!");
            return;
        }

        //wheel inventory
        if (index > weapons.Length - 1)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = weapons.Length - 1;
        }

        switching = true;

        //hoster old weapon
        weaponIndex = index;
        animator.SetTrigger("hoster");
    }

    //animation event trigger
    public void FinishedHostered()
    {
        //disable old weapon
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        //enable new weapon when unhostering
        currentWeapon = weapons[weaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    //animation event trigger
    public void ResetEquip()
    {
        switching = false;
    }
}
