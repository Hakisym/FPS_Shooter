using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack",menuName = "Enemy/Attack/Attack Profile")]
public class Attack : ScriptableObject
{
    public AnimationClip animationClip;
    public int animationIndex;
    public int regularDamage;
    public float engageRange;
    public float knockForce;
}
