using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount, DamageInfo info = null) { }
}

public class DamageInfo
{
    public float force;
    public Vector3 direction;
    public Collider bodyPart;

    public DamageInfo(float force,Vector3 direction, Collider bodypart)
    {
        this.force = force;
        this.direction = direction;
        this.bodyPart = bodypart;
    }
}
