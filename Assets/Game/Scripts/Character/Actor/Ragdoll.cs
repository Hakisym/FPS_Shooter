using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    private void Start()
    {
        //Disable ragdoll physics
        foreach (var member in GetComponentsInChildren<Rigidbody>())
        {
            member.isKinematic = true;
        }
    }

    public void RagdollForce(Collider bodyPart,Vector3 deathForce)
    {
        bodyPart.GetComponent<Rigidbody>().AddForce(deathForce, ForceMode.Impulse);
    }

    public void EnableRagdoll()
    {
        RemoveAllForces();
        GetComponent<Animator>().enabled = false;
        GetComponent<Collider>().enabled = false;
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
