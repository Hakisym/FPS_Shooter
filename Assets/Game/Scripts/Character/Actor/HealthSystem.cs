using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public float destroyDelay;
    public bool dead;
    public UnityEvent OnDead;

    public int currentHealth { get; set; }
    private float headshotMultiplier = 1.5f;
    private float ragdollShootForce = 100f;

    private Ragdoll ragdoll;
    private Actor actor;

    private void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<Ragdoll>();
        actor = GetComponent<Actor>();
    }

    public void TakeDamage(int amount, DamageInfo info)
    {
        if (info.type == DamageType.bullet)
        {
            //critical body part
            if (info.bodyPart.CompareTag("Head"))
            {
                currentHealth -= (int)(amount * headshotMultiplier);
            }
            else
            {
                currentHealth -= amount;
            }
        }
        else
            currentHealth -= amount;

        //check health below 0
        if (currentHealth <= 0)
        {
            if (!dead)
            {
                //confirm actor kill
                info.source?.onKill(this.GetComponent<Actor>());
                Die();
                if (ragdoll) ragdoll.RagdollForce(info.bodyPart, info.deathForce);
            }
            else
                if (ragdoll) ragdoll.RagdollForce(info.bodyPart, info.deathForce.normalized * ragdollShootForce);
        }

        if (actor != null)
            if(actor.isLocallyMine) UIManager.instance.OnReceiveDamage();
    }

    private void Die()
    {
        OnDead?.Invoke();

        //disable controller and navmesh
        //foreach (var mb in this.GetComponents<MonoBehaviour>())
        //{
        //    if (this != mb)
        //        Destroy(mb);
        //}
        //GetComponent<NavMeshAgent>().enabled = false;

        Destroy(gameObject, destroyDelay);

        Respawn();

        if (ragdoll) ragdoll.EnableRagdoll();
        dead = true;
    }

    private void Respawn()
    {
        if (actor.actorManager && actor.actorManager.respawnable) actor.actorManager.Respawn();
    }

    public bool IsDead()
    {
        return dead;
    }
}
