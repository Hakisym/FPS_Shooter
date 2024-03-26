using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    public Action OnGrabSuccess;
    private ZombieStateController enemy;

    private void Start()
    {
        enemy = transform.root.GetComponent<ZombieStateController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            if (pm.canMove)
            {
                OnGrabSuccess?.Invoke();
                pm.canMove = false;
            }
        }
    }
}
