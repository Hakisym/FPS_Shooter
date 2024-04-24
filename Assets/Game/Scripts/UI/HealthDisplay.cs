using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    public TextMeshProUGUI healthReserve;
    private HealthSystem healthSystem;

    private void Update()
    {
        healthReserve.text = healthSystem.currentHealth.ToString();
    }

    public void GetReference(HealthSystem ph)
    {
        this.healthSystem = ph;
    }
}
