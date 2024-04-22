using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public int maxHealth;
    public int currentHealth;
    private ImpactManager impactManager;

    private void Start()
    {
        currentHealth = maxHealth;
        impactManager = GetComponent<ImpactManager>();
    }

    public void TakeDamage(int amount, DamageInfo info)
    {
        UIManager.instance.OnReceiveDamage();

        currentHealth -= amount;

        if(info!=null) impactManager.AddImpact(info.direction, info.force);

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }
}
