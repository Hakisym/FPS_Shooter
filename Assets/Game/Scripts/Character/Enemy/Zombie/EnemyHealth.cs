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
    public float ragdollShootForce = 100f;

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

    public void TakeDamage(int amount, DamageInfo info)
    {
        //Count body and head HP individually
        if (info.bodyPart.CompareTag("Head"))
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
                info.bodyPart.GetComponent<Rigidbody>().AddForce(info.direction.normalized * info.force, ForceMode.Impulse);
            }
            else info.bodyPart.GetComponent<Rigidbody>().AddForce(info.direction.normalized * ragdollShootForce, ForceMode.Impulse);
        }

        if (!dead) UIController.instance.OnEnemyHit.Invoke();
    }

    private void Die()
    {
        OnDie?.Invoke();

        UIController.instance.OnEnemyKill.Invoke();

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
