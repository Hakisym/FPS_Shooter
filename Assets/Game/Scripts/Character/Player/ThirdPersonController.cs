using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ThirdPersonController : MonoBehaviour
{
    public Actor Actor { get; set; }
    private Animator animator;
    private CharacterManager characterManager;
    private PlayerMovement pm;
    private WeaponController wc;
    private SkinnedMeshRenderer meshRenderer;
    private Ragdoll ragdoll;

    public Rig aimRig;
    public MultiAimConstraint chestConstrain;
    public MultiAimConstraint upperChestConstrain;

    public Transform handIk;
    public Transform aimTarget;
    public Transform rightHandTarget;

    public float speedChangeRate;
    public float fallTimeout;

    public Vector3 aimConstrainOffset;
    public Vector3 handTargetPos;
    private Vector3 _originalRigOffset;

    private Vector2 targetMove;
    private float _animationBlend;
    private float _fallTimeoutDelta;

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        //if (Actor.isLocallyMine) meshRenderer.enabled = false;

        animator = GetComponent<Animator>();
        ragdoll = GetComponent<Ragdoll>();

        _originalRigOffset = chestConstrain.data.offset;
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

            //destroy current weapon
            foreach (Transform child in handIk) 
                Destroy(child.gameObject);
            var obj = Instantiate(wd.pickUpGraphics, handIk);
            // Destroy all colliders
            obj.GetComponentsInChildren<Collider>().ToList().ForEach(childCollider => Destroy(childCollider));
        };

        wc.OnFire.AddListener(() => animator.SetTrigger("Fire"));
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

        bool ikEnable = wc.aiming || (move == Vector2.zero && wc.wd);

        //rig weight
        var _aimRigWeight = ikEnable ? 1 : 0;
        aimRig.weight = Mathf.Lerp(aimRig.weight, _aimRigWeight, Time.deltaTime * speedChangeRate);
        //constrain offset
        var _bodyRigOffset = ikEnable ? aimConstrainOffset : _originalRigOffset;
        chestConstrain.data.offset = Vector3.Lerp(chestConstrain.data.offset, _bodyRigOffset, Time.deltaTime * speedChangeRate);
        upperChestConstrain.data.offset = Vector3.Lerp(upperChestConstrain.data.offset, _bodyRigOffset, Time.deltaTime * speedChangeRate);


        if (characterManager == null) return;

        //sync horizontal position
        transform.position = Vector3.Lerp(transform.position, new Vector3(characterManager.transform.position.x, transform.position.y, characterManager.transform.position.z) , Time.deltaTime * 10);
        //sync vertical position
        transform.position = new Vector3(transform.position.x, characterManager.transform.position.y, transform.position.z);
        //sync rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, characterManager.transform.rotation, Time.deltaTime * 10);

        aimTarget.position = characterManager.aimTarget.position;
    }

    private void JumpManagement()
    {
        animator.SetTrigger("Jump");
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
