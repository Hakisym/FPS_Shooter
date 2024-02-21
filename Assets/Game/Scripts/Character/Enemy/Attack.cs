using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack",menuName = "Enemy/Attack/AttackRegular")]
public class Attack : ScriptableObject
{
    public AnimationClip animationClip;
    public int animationIndex;
    public int regularDamage;
    public float range;
    public float knockForce;
}
