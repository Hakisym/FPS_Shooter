using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour,IDamageable
{
    public int maxHealth;

    private int bodyHealth;
    private int headHealth;

    private void Start()
    {
        bodyHealth = maxHealth;
        headHealth = (int)(maxHealth / 3f);
    }

    public void TakeDamage(int amount, float force, Vector3 direction, Collider bodyPart)
    {
        if (bodyPart.CompareTag("Head"))
        {
            headHealth -= amount;
        }
        else
        {
            bodyHealth -= amount;
        }
        if (headHealth <= 0 || bodyHealth <= 0)
        {
            Die(force, direction, bodyPart);
        }
    }

    private void Die(float force,Vector3 direction,Collider bodyPart)
    {
        GetComponent<Animator>().enabled = false;
        bodyPart.GetComponent<Rigidbody>().AddForce(direction.normalized * force, ForceMode.Impulse);
    }
}
