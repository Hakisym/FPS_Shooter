using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public Rigidbody[] rigidbodies { get; set; }
    public Animator animator;


    private void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        if (!animator) animator = GetComponent<Animator>();

        foreach (var rb in rigidbodies)
        {
            //rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        Disable();
    }

    public void Enable()
    {
        if (animator) animator.enabled = false;

        //Enable ragdoll
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
    }

    public void Enable(Vector3 deathForce)
    {
        if (animator) animator.enabled = false;

        //Enable ragdoll
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;

            rb.AddForce(deathForce, ForceMode.VelocityChange);
        }
    }

    public void Disable()
    {
        if (animator) animator.enabled = true;

        //Disable ragdoll physics
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
    }
}
