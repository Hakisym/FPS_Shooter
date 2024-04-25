using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>().TakeDamage(40, null);

            other.GetComponent<IDamageable>().deathForce = (other.transform.position - transform.position).normalized * 10f;
        }
    }
}
