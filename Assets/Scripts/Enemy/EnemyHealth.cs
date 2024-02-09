using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : HealthManager
{
    public int maxHealth;
    public bool dead;

    private int bodyHealth;
    private int headHealth;

    private void Start()
    {
        bodyHealth = maxHealth;
        headHealth = (int)(maxHealth / 3f);

        //Disable ragdoll physics
        foreach (var member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = true;
        }
    }

    public override void TakeDamage(int amount, float force, Vector3 direction, Collider bodyPart)
    {
        //Count body and head HP individually
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
        //Destroy enemy scripts and navmesh
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
        //Enable ragdoll
        foreach (var member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = false;
            member.velocity = Vector3.zero;
        }
    }
}
