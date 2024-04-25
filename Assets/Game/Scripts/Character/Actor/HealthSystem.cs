using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float destroyDelay;
    public float destroyRagdollDelay;
    public bool dead;
    public UnityEvent OnDead;

    public float currentHealth { get; set; }

    public Actor Actor { get; set; }
    public bool deadConfirmed { get; set; }
    public Vector3 deathForce { get; set; }
    private Actor killer;

    public Ragdoll ragdoll;
    private DeathCamera deathcamera;

    private void Start()
    {
        currentHealth = maxHealth;

        if (!ragdoll) ragdoll = GetComponent<Ragdoll>();
        Actor = GetComponent<Actor>();

        if (FindObjectOfType<GameManager>()) deathcamera = FindObjectOfType<GameManager>().DeathCamera;
        if (Actor && Actor.isLocallyMine) deathcamera?.Disable(); 

    }

    public void TakeDamage(float amount, Actor source)
    {
        currentHealth -= amount;
        this.killer = source;

        //check health below 0
        if (currentHealth <= 0)
        {
            if (!dead)
                Die();
        }

        if (Actor != null && Actor.isLocallyMine) UIManager.instance.OnReceiveDamage();
    }

    private void Die()
    {
        OnDead?.Invoke();

        ////disable controller and navmesh
        //foreach (var mb in this.GetComponents<MonoBehaviour>())
        //{
        //    if (mb != this && mb!=actor)
        //        Destroy(mb);
        //}
        //Destroy(GetComponent<NavMeshAgent>());

        GetComponent<Collider>().enabled = false;

        if (ragdoll.transform != this.transform)
        {
            ragdoll.transform.parent = transform.parent;
            Destroy(ragdoll.gameObject, destroyRagdollDelay);
        }
        
        Destroy(gameObject, destroyDelay);
        if (ragdoll) ragdoll.Enable(deathForce);

        Respawn();

        dead = true;
    }

    private void Respawn()
    {
        if (Actor.actorManager && Actor.actorManager.respawnable) Actor.actorManager.Respawn();

        if (Actor.isLocallyMine) deathcamera.Enable();
        if (Actor.isLocallyMine) ragdoll.GetComponent<ThirdPersonController>().Enable();

    }

    public bool IsDead()
    {
        return dead;
    }

    public Actor GetActor()
    {
        return Actor;
    }
}
