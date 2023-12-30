using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public bool isAiming;
    public float aimEffectMultiplier = 0.2f;
    public float currentAimEffectMultiplier;

    private void Start()
    {
        currentAimEffectMultiplier = aimEffectMultiplier;
    }

    private void Update()
    {
        currentAimEffectMultiplier = isAiming ? aimEffectMultiplier : 1f;
    }
}
