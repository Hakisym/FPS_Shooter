using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int amount,DamageInfo info);
    public bool IsDead();
}

public class DamageInfo
{
    public DamageType type;
    public Vector3 deathForce;
    public Collider bodyPart;
    public Actor source;

    public DamageInfo(DamageType type, Vector3 force, Collider bodypart, Actor damageSource)
    {
        this.type = type;
        this.deathForce = force;
        this.bodyPart = bodypart;
        this.source = damageSource;
    }
}

public enum DamageType
{
    bullet,
    melee,
    trap
}
