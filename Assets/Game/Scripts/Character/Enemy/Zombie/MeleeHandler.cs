using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHandler : MonoBehaviour
{
    public Attack attack;
    private StateController enemy;

    private void Start()
    {
        enemy = transform.root.GetComponent<StateController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthManager>().TakeDamage(attack.regularDamage, attack.knockForce, other.transform.position - enemy.transform.position);
        }
    }
}
