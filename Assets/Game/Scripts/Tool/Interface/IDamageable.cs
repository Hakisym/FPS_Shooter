using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float amount,Actor source);
    public Actor GetActor();
    public bool IsDead();
    public bool deadConfirmed { get; set; }
    public Vector3 deathForce { get; set; }
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
