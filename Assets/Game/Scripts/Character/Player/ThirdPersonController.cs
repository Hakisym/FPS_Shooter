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

    public float speedChangeRate;
    public float fallTimeout;

    private Transform hips, spine, chest;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;
    private Quaternion lastRotation;

    private Vector2 targetMove;
    private float _animationBlend;
    private float _fallTimeoutDelta;

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

    private void Start()
    {
        pm.OnJump.AddListener(JumpManagement);
        wc.inventory.OnEquipWeapon += () => animator.SetTrigger("ChangeWeapon");
    }

    private void Update()
    {
        float inputRawX = Input.GetAxisRaw("Horizontal");
        float inputRawY = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(inputRawX, inputRawY);

        //reset speed when no input
        var targetSpeed = move == Vector2.zero ? 0 : pm.currentSpeed;
        //smooth the speed curve
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;
        animator.SetFloat("Speed", _animationBlend);

        //smooth the input vector
        targetMove = Vector2.Lerp(targetMove, move, Time.deltaTime * speedChangeRate);
        if (targetMove.magnitude < 0.01f) targetMove = Vector2.zero;
        animator.SetFloat("Horizontal", targetMove.x);
        animator.SetFloat("Vertical", targetMove.y);

        animator.SetBool("Crouching", pm.isCrouching);
        animator.SetBool("Grounded", pm.isGrounded);

        if (pm.isGrounded)
        {
            //reset fall time
            _fallTimeoutDelta = fallTimeout;
            animator.SetBool("Jump", false);
            animator.SetBool("FreeFall", false);
        }
        else
        {
            //set fall animation time
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                animator.SetBool("FreeFall", true);
            }
        }

        animator.SetInteger("Weapon", (wc.wd) ? 1 : 0);
        animator.SetBool("Aim", wc.aiming);


    }

    private void LateUpdate()
    {
        Quaternion targetRotation = Quaternion.LookRotation(wc.playerCam.transform.forward);

        //calculate bone transform(root->hips->spine)
        targetRotation *= Quaternion.Euler(initialRootRotation);
        targetRotation *= Quaternion.Euler(initialHipsRotation);
        targetRotation *= Quaternion.Euler(initialSpineRotation);

        //limit spine rotation
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

    private void JumpManagement()
    {
        animator.SetBool("Jump", true);
    }

    public void Enable()
    {
        meshRenderer.enabled = true;
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f) { }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f) { }
    }
}
