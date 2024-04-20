using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public bool dead;
    public Action OnDie;

    private int currentHealth;
    private float ragdollShootForce = 100f;
    private float headshotMultiplier = 1.5f;

    private void Start()
    {
        currentHealth = maxHealth;

        //Disable ragdoll physics
        foreach (var member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = true;
        }
    }

    public void TakeDamage(int amount, DamageInfo info)
    {
        //Count body and head HP individually
        if (info.bodyPart.CompareTag("Head"))
        {
            currentHealth -= (int)(amount * headshotMultiplier);
        }
        else
        {
            currentHealth -= amount;
        }

        if (currentHealth <= 0)
        {
            if (!dead)
            {
                UIManager.instance.OnEnemyHit.Invoke(true,true);
                Die();
                info.bodyPart.GetComponent<Rigidbody>().AddForce(info.direction.normalized * info.force, ForceMode.Impulse);
            }
            else
            {
                UIManager.instance.OnEnemyHit.Invoke(false, false);
                info.bodyPart.GetComponent<Rigidbody>().AddForce(info.direction.normalized * ragdollShootForce, ForceMode.Impulse);
            }
        }
        else
            UIManager.instance.OnEnemyHit.Invoke(false,true);
    }

    private void Die()
    {
        OnDie?.Invoke();

        UIManager.instance.OnEnemyKill.Invoke();

        //Destroy enemy scripts and navmesh
        foreach (var mb in this.GetComponents<MonoBehaviour>())
        {
            if (this != mb)
                Destroy(mb);
        }
        Destroy(this.GetComponent<NavMeshAgent>());
        RemoveAllForces();
        GetComponent<Animator>().enabled = false;
        GetComponent<Collider>().enabled = false;
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
