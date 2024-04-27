using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Actor Actor { get; set; }
    private Animator animator;
    private PlayerMovement pm;
    private WeaponController wc;
    private SkinnedMeshRenderer meshRenderer;

    private Transform hips, spine, chest;
    public float speedChangeRate;
    private float animationBlend;
    private Vector2 targetMove;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;
    private Quaternion lastRotation;

    private void Awake()
    {
        Actor = GetComponentInParent<Actor>();

        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //if (Actor.isLocallyMine) meshRenderer.enabled = false;

        animator = GetComponent<Animator>();
        pm = GetComponentInParent<PlayerMovement>();
        wc = GetComponentInParent<WeaponController>();

        //get avatar bones
        hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        Transform root = hips.parent;

        initialRootRotation = root.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;
        initialChestRotation = chest.localEulerAngles;
    }

    private void Update()
    {
        float inputRawX = Input.GetAxisRaw("Horizontal");
        float inputRawY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(inputRawX, inputRawY);

        var targetSpeed = move == Vector2.zero ? 0 : pm.currentSpeed;
        animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);

        animator.SetFloat("Speed", animationBlend);

        targetMove = Vector2.Lerp(targetMove, move, Time.deltaTime * speedChangeRate);
        animator.SetFloat("Horizontal", targetMove.x);
        animator.SetFloat("Vertical", targetMove.y);
        animator.SetBool("Crouching", pm.isCrouching);
        animator.SetBool("Grounded", pm.isGrounded);
    }

    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(wc.playerCam.transform.forward);

        targetRotation *= Quaternion.Euler(initialRootRotation);
        targetRotation *= Quaternion.Euler(initialHipsRotation);
        targetRotation *= Quaternion.Euler(initialSpineRotation);

        if (Quaternion.Angle(targetRotation, hips.rotation) <= 60f)
        {
            chest.rotation = targetRotation * Quaternion.Euler(initialChestRotation);
            lastRotation = chest.localRotation;
        }
        else
        {
            chest.localEulerAngles =  new Vector3(lastRotation.eulerAngles.x, chest.localEulerAngles.y, chest.localEulerAngles.z);
        }

    }

    public void Enable()
    {
        meshRenderer.enabled = true;
    }
}
