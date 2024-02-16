using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : HealthManager
{
    public int maxHealth;
    public int currentHealth;
    private ImpactManager impactManager;

    private void Start()
    {
        currentHealth = maxHealth;
        impactManager = GetComponent<ImpactManager>();
    }

    public override void TakeDamage(int amount, float force, Vector3 direction, Collider bodyPart = null)
    {
        currentHealth -= amount;
        impactManager.AddImpact(direction, force);
        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }
}
