using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public bool Crosshair;

    public CrossHair crosshair;
    public WeaponHUD weaponHUD;
    public HealthDisplay healthDisplay;
    
    private void Update()
    {
        crosshair.enableCrosshair = Crosshair;
    }
}
