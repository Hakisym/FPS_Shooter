using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponHUD : MonoBehaviour
{
    public TextMeshProUGUI ammoReserve;
    public TextMeshProUGUI ammoAmount;
    private WeaponController weaponController;

    void Update()
    {
        if (!weaponController.preset) return;

        ammoReserve.text = weaponController.bulletsLeft.ToString();
        ammoAmount.text = weaponController.preset.gunStats.magazineSize.ToString();
    }

    public void GetReference(WeaponController wc)
    {
        this.weaponController = wc;
    }
}
