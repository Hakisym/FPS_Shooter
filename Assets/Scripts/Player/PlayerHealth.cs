using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable
{
    public int maxHealth;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount, float force, Vector3 direction, Collider bodyPart)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }
}
