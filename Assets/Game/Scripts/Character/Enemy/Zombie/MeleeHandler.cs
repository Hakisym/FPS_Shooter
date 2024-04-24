using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHandler : MonoBehaviour
{
    public Attack attack;
    private ZombieStateController enemy;

    private void Start()
    {
        enemy = transform.root.GetComponent<ZombieStateController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var dir = other.transform.position - enemy.transform.position;
            DamageInfo info = new DamageInfo(DamageType.melee, dir * attack.knockForce, other, null);
            other.GetComponent<IDamageable>().TakeDamage(attack.regularDamage, info);
            other.GetComponent<ImpactManager>()?.AddImpact(dir, attack.knockForce);
        }
    }
}
