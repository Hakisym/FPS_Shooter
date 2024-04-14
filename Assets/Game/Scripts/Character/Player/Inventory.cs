using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<WeaponIdentification> weapons;
    public WeaponIdentification currentWeapon;
    public int inventorySize;
    public int initialWeapon = 0;
    public float dropForce = 10f;
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
        weapons = new List<WeaponIdentification>();

        weapons = GetComponentsInChildren<WeaponIdentification>(true).ToList();

        // equip at start
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
        Equip(initialWeapon);
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        // Check if there's a valid scroll input
        if (scrollValue != 0f)
        {
            // Check if enough time has passed since the last switch
            if (Time.time - lastSwitchTime >= switchDelay)
            {
                if (weapons.Count <= 1)
                {
                    Debug.Log("Can't switch with one weapon!");
                    return;
                }

                // Switch weapon
                if (scrollValue > 0) Equip(weaponIndex + 1);
                else Equip(weaponIndex - 1);

                // Update the last switch time
                lastSwitchTime = Time.time;
            }
        }
    }

    public void Equip(int index)
    {
        //state check
        if (switching) return;

        if (currentWeapon != null && (wc.reloading || wc.shootState))
        {
            Debug.Log("Can't switch when reloading and shooting!");
            return;
        }

        //wheel inventory
        if (index > weapons.Count - 1)
        {
            index = 0;
        }
        if (index < 0)
        {
            index = weapons.Count - 1;
        }

        switching = true;
        weaponIndex = index;

        //if currently holding a gun
        if (currentWeapon != null)
        {
            //hoster current gun
            animator.SetTrigger("hoster");
            //play hoster sound
            wc.mainAudioSource.clip = currentWeapon.wd.soundClips.holsterSound;
            wc.mainAudioSource.Play();
        }
        //if currently empty
        else
        {
            //take out new weapon
            TakeOutWeapon();
            //play take out animation
            animator.SetTrigger("unhoster");
        }
        
    }

    //take out new weapon
    public void TakeOutWeapon()
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

        //play take out sound
        wc.mainAudioSource.clip = currentWeapon.wd.soundClips.takeOutSound;
        wc.mainAudioSource.Play();
    }

    //when finish taking out weapon
    public void ResetEquip()
    {
        switching = false;
        wc.CanShoot();
    }

    public bool PickupWeapon(WeaponData weaponSO)
    {
        //create new gun
        var weaponGO = Instantiate(weaponSO.gunPrefab, this.transform);
        var newWeapon = weaponGO.GetComponent<WeaponIdentification>();
        //add to inventory and hide it
        weapons.Add(newWeapon);
        newWeapon.gameObject.SetActive(false);

        return true;
    }

    public void DropGun()
    {
        //one gun remaining
        if (switching || weapons.Count <= 1) return;

        //spawn pickup GO
        var pickupGO = Instantiate(pickupPrefab,transform.position,Quaternion.identity);
        pickupGO.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * dropForce, ForceMode.Impulse);
        pickupGO.GetComponent<WeaponPickable>().weaponData = currentWeapon.wd;

        //delete from inventory
        weapons.Remove(currentWeapon);
        //detach muzzle flash
        wc.muzzleFlash.GetComponent<WeaponMuzzleFlash>().DetachMuzzleFlash(this.transform);

        Destroy(currentWeapon.gameObject);
        currentWeapon = null;

        Equip(weaponIndex + 1);
    }
}
