using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponHUD : MonoBehaviour
{
    [SerializeField] TMP_Text ammoReserve;

    public void SetAmmoValue(int value,bool show = true)
    {
        if (show)
        {
            ammoReserve.text = value.ToString();
            if(!gameObject.activeSelf) gameObject.SetActive(true);
        }
        else
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
    }
}
