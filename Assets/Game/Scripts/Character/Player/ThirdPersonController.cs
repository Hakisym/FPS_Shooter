using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement pm;
    public float speedChangeRate;
    private float animationBlend;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pm = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        var targetSpeed = pm.moveVel == 0 ? 0 : pm.currentSpeed;
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        animator.SetFloat("Speed", animationBlend);
    }
}
