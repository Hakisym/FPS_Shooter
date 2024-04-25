using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Actor Actor { get; set; }
    private Animator animator;
    private PlayerMovement pm;
    public float speedChangeRate;
    private float animationBlend;
    private SkinnedMeshRenderer meshRenderer;

    private void Awake()
    {
        Actor = GetComponentInParent<Actor>();

        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (Actor.isLocallyMine) meshRenderer.enabled = false;

        animator = GetComponent<Animator>();
        pm = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var targetSpeed = move == Vector2.zero ? 0 : pm.currentSpeed;
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        animator.SetFloat("Speed", animationBlend);
    }

    public void Enable()
    {
        meshRenderer.enabled = true;
    }
}
