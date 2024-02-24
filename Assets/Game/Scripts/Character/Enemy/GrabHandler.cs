using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
    public Action OnGrabSuccess;
    private StateController enemy;

    private void Start()
    {
        enemy = transform.root.GetComponent<StateController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var pm = other.GetComponent<PlayerMovement>();
            if (!pm.isGrabbed)
            {
                OnGrabSuccess?.Invoke();
                pm.isGrabbed = true;
            }
        }
    }
}
