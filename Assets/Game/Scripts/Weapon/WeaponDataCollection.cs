using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDataDefaultList", menuName = "Weapon/WeaponDataCollection")]
public class WeaponDataCollection : ScriptableObject
{
    public List<WeaponData> weapons;

    public WeaponData GetWeaponData(int id)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.gunID == id) return weapon;
        }
        Debug.Log("No such weapon ID has been found in prefab list, please add it into weapon list SO.");
        return null;
    }
}
