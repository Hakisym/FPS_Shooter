using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public virtual void TakeDamage(int amount, float force, Vector3 direction, Collider bodyPart=null) { }
}
