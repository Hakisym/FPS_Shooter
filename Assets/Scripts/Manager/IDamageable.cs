using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount, float force, Vector3 direction, Collider bodyPart);
}
