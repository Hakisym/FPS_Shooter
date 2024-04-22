using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    public TextMeshProUGUI healthReserve;
    private PlayerHealth playerHealth;

    private void Update()
    {
        healthReserve.text = playerHealth.currentHealth.ToString();
    }

    public void GetReference(PlayerHealth ph)
    {
        this.playerHealth = ph;
    }
}
