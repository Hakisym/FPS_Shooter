using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public float damage = 50f;
    public float force = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>().TakeDamage(damage, null);

            other.GetComponent<IDamageable>().deathForce = (other.transform.position - transform.position).normalized * force;
        }
    }
}
