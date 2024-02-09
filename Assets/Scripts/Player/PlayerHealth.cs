using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthManager
{
    public int maxHealth;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public override void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }
}
