using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public bool isAiming;
    public float aimSwayMultiplier = 0.2f;
    public float aimBobbingMultiplier = 0.1f;
    public float currentAimSwayMultiplier;
    public float currentAimBobbingMultiplier;

    private void Update()
    {
        if (isAiming)
        {
            currentAimBobbingMultiplier = aimBobbingMultiplier;
            currentAimSwayMultiplier = aimSwayMultiplier;
        }
        else
        {
            currentAimBobbingMultiplier = 1f;
            currentAimSwayMultiplier = 1f;
        }
    }
}
