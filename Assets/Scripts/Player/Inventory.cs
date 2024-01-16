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
        foreach (var weapon in weapons)
        {
            if (weapon.gameObject.activeSelf && currentWeapon==null)
            {
                currentWeapon = weapon;
            }
            else
            {
                weapon.gameObject.SetActive(false);
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
                // Switch weapon
                if(scrollValue>0) Equip(++weaponIndex);
                else Equip(--weaponIndex);

                // Update the last switch time
                lastSwitchTime = Time.time;
            }
        }
    }

    public void Equip(int index)
    {
        if (switching) return;

        if (currentWeapon != null && (currentWeapon.reloading || currentWeapon.shooting))
        {
            Debug.Log("Can't switch when reloading and shooting!");
            return;
        }

        if (index > weapons.Length - 1)
        {
            index = 0;
        }

        if (index < 0)
        {
            index = weapons.Length - 1;
        }

        switching = true;

        weaponIndex = index;
        animator.SetTrigger("hoster");
    }

    public void FinishedHostered()
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapons[weaponIndex];
        currentWeapon.gameObject.SetActive(true);
        // Set up recoil and aim offset
    }

    public void ResetEquip()
    {
        switching = false;
    }
}
