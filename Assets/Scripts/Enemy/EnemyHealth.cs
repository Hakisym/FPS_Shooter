using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour,IDamageable
{
    public int maxHealth;
    public bool dead;

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
            if (!dead)
            {
                Die();
            }
            bodyPart.GetComponent<Rigidbody>().AddForce(direction.normalized * force, ForceMode.Impulse);
        }
    }

    private void Die()
    {
        foreach (var mb in this.GetComponents<MonoBehaviour>())
        {
            if (this != mb)
                Destroy(mb);
        }
        Destroy(this.GetComponent<NavMeshAgent>());
        RemoveAllForces();
        GetComponent<Animator>().enabled = false;
        dead = true;
    }

    private void RemoveAllForces()
    {
        foreach (var member in GetComponentsInChildren<Rigidbody>())
        {
            member.velocity = Vector3.zero;
        }
    }
}
