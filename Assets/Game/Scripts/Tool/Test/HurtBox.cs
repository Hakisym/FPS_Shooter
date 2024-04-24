using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>().TakeDamage(10, new DamageInfo(DamageType.trap, Vector3.zero, null, null));
        }
    }
}
