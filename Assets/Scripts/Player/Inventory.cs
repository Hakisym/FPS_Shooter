using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public WeaponIdentification[] weapons;
    public WeaponIdentification currentWeapon;
    public int inventorySize;
    public int initialWeapon = 0;
    private int weaponIndex = -1;

    [Header("Animation")]
    public Animator animator;
    public bool switching;
    public float switchDelay = 1.0f;  // Adjust the delay time as needed
    private float lastSwitchTime;

    [Header("Reference")]
    public GameObject pickupPrefab;
    public WeaponController wc;
    public static Inventory instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        weapons = new WeaponIdentification[inventorySize];

        weapons = GetComponentsInChildren<WeaponIdentification>(true);

        // equip at start
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        Equip(initialWeapon);
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
        if (currentWeapon != null && (wc.reloading || wc.shooting))
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

        weaponIndex = index;
        if (currentWeapon != null)
        {
            //hoster old weapon
            animator.SetTrigger("hoster");
        }
        else
        {
            FinishedHostered();
            //simply unhoster new weapon
            animator.SetTrigger("unhoster");
        }
        
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
        wc.GetWeaponData(currentWeapon);
    }

    //animation event trigger
    public void ResetEquip()
    {
        switching = false;
    }

    public bool SwapWeapon(WeaponData weaponSO)
    {
        if (switching) return false;
        //create new gun
        var weaponGO = Instantiate(weaponSO.gunPrefab, this.transform);
        var newWeapon = weaponGO.GetComponent<WeaponIdentification>();

        weapons[weaponIndex] = newWeapon;

        //drop old gun
        var pickupGO = Instantiate(pickupPrefab);
        pickupGO.GetComponent<WeaponPickable>().weaponData = currentWeapon.wd;

        //clean up
        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
        
        Equip(weaponIndex);
        return true;
    }
}
