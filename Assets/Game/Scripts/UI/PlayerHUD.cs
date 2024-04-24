using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public bool Crosshair;

    [HideInInspector] public CrossHair crosshair;
    [HideInInspector] public WeaponHUD weaponHUD;
    [HideInInspector] public HealthDisplay healthDisplay;
    
    [HideInInspector] public WeaponController wc;

    private void Awake()
    {
        wc = GetComponentInParent<WeaponController>();

        crosshair = GetComponentInChildren<CrossHair>();
        crosshair.GetReference(wc, wc.GetComponent<PlayerMovement>());

        weaponHUD = GetComponentInChildren<WeaponHUD>();
        weaponHUD.GetReference(wc);

        healthDisplay = GetComponentInChildren<HealthDisplay>();
        healthDisplay.GetReference(wc.GetComponent<HealthSystem>());
    }

    private void Update()
    {
        crosshair.enableCrosshair = Crosshair;
    }
}
