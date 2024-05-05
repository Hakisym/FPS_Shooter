using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public Actor Actor { get; set; }
    private Animator animator;
    private CharacterManager characterManager;
    private PlayerMovement pm;
    private WeaponController wc;
    private SkinnedMeshRenderer meshRenderer;
    private Ragdoll ragdoll;

    public Transform ikHandGun;
    public Transform ikRotation;
    public Transform ikRotationAim;
    public float speedChangeRate;
    public float fallTimeout;

    private Transform hips, spine, chest;
    private Vector3 initialRootRotation;
    private Vector3 initialHipsRotation;
    private Vector3 initialSpineRotation;
    private Vector3 initialChestRotation;
    private Quaternion lastRotation;

    private Vector3 _targetDirection;
    private Vector2 targetMove;
    private float _animationBlend;
    private float _fallTimeoutDelta;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //if (Actor.isLocallyMine) meshRenderer.enabled = false;

        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();

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

    public void Setup(CharacterManager _characterManager)
    {
        characterManager = _characterManager;
        Actor = _characterManager.actor;
        pm = _characterManager.playerMovement;
        wc = _characterManager.weaponController;

        //setup ragdoll
        HealthSystem healthSystem = (HealthSystem)characterManager.damageable;
        healthSystem.ragdoll = this.ragdoll;
    }

    private void Start()
    {
        pm.OnJump.AddListener(JumpManagement);
        wc.inventory.OnEquipWeapon += (WeaponData wd) =>
        {
            animator.SetTrigger("ChangeWeapon");
            Instantiate(wd.pickUpGraphics, ikHandGun);
        };
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
        //Vector3 playerHeadEulerAngles = wc.aiming ? ikRotationAim.rotation.eulerAngles : ikRotation.rotation.eulerAngles;
        //playerHeadEulerAngles.x = ModularClamp(playerHeadEulerAngles.x, 60, 300);
        //Quaternion playerHead = Quaternion.Euler(playerHeadEulerAngles);

        //Vector3 playerHeadForward = playerHead * Vector3.forward;
        //playerHeadForward.Normalize();

        //_targetDirection = Vector3.Lerp(_targetDirection, playerHeadForward, Time.deltaTime * speedChangeRate);

        //Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);

        ////calculate bone transform(root->hips->spine)
        //targetRotation *= Quaternion.Euler(initialRootRotation);
        //targetRotation *= Quaternion.Euler(initialHipsRotation);
        //targetRotation *= Quaternion.Euler(initialSpineRotation);

        //chest.rotation = targetRotation * Quaternion.Euler(initialChestRotation);
    }

    float ModularClamp(float val, float min, float max)
    {
        if (val <= 180)
        {
            if (val > min) val = min;
        }
        else
        {
            if (val < max) val = max;
        }
        return val;
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
