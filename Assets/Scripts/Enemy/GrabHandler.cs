using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHandler : MonoBehaviour
{
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
                enemy.animator.SetInteger("attack", -1);
                enemy.grabbedTarget = true;
                pm.isGrabbed = true;
            }
        }
    }
}
