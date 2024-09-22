using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

public class ThirdPersonController : NetworkBehaviour
{
    public WeaponDataCollection weaponPrefabs;
    private Animator animator;
    private NetworkAnimator networkAnimator;
    private NetworkCharacter characterManager;
    private PlayerMovement pm;
    private WeaponController wc;
    private SkinnedMeshRenderer meshRenderer;
    private Ragdoll ragdoll;

    private GameObject bulletCasting;
    private Transform prefabHolder;
    private Animator gunAnim;
    private ParticleSystem muzzleVFX;
    private Transform castPoint;

    public Rig aimRig;

    public Transform handIk;
    public Transform aimTarget;
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    public float speedChangeRate;
    public float fallTimeout;

    public Vector3 rightHandTargetPos;
    public Vector3 rightHandTargetAim;
    public Vector3 leftHandTargetPos;

    private Vector2 targetMove;
    private float _animationBlend;
    private float _fallTimeoutDelta;

    public bool showLocalPlayer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            characterManager = GetComponentInParent<NetworkCharacter>();
            Setup(characterManager);
            Initialize();

            if (!showLocalPlayer)
                meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
        ragdoll = GetComponent<Ragdoll>();

        rightHandTargetPos = rightHandTarget.localPosition;
    }

    public void Setup(NetworkCharacter _characterManager)
    {
        characterManager = _characterManager;
        pm = _characterManager.playerMovement;
        wc = _characterManager.weaponController;

        //setup ragdoll
        //HealthSystem healthSystem = (HealthSystem)characterManager.damageable;
        //healthSystem.ragdoll = this.ragdoll;
    }

    private void Initialize()
    {
        prefabHolder = FindObjectOfType<PoolManager>().poolHolder;

        pm.OnJump.AddListener(JumpManagement);

        characterManager.inventory.OnEquipWeapon.AddListener(EquipWeapon);

        characterManager.inventory.OnDropWeapon.AddListener(DropWeapon);

        wc.OnShoot.AddListener(FireWeapon);
    }

    private void Update()
    {
        if (base.IsOwner)
        {

            float inputRawX = Input.GetAxisRaw("Horizontal");
            float inputRawY = Input.GetAxisRaw("Vertical");
            Vector2 move = new Vector2(inputRawX, inputRawY);

            //smooth the speed curve
            _animationBlend = Mathf.Lerp(_animationBlend, pm.currentSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            animator.SetFloat("Speed", _animationBlend);

            //smooth the input vector
            targetMove = Vector2.Lerp(targetMove, move, Time.deltaTime * speedChangeRate);
            if (targetMove.magnitude < 0.01f) targetMove = Vector2.zero;
            animator.SetFloat("Horizontal", targetMove.x);
            animator.SetFloat("Vertical", targetMove.y);

            animator.SetBool("Crouching", pm.moveState == MoveState.Crouching);
            bool isGrounded = pm.moveState != MoveState.InAir;
            animator.SetBool("Grounded", isGrounded);

            if (isGrounded)
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

            animator.SetInteger("Weapon", (wc.preset) ? 1 : 0);
            animator.SetBool("Aim", wc.aiming);

            // enable ik if aiming,idle or shooting
            bool ikEnable = wc.aiming || (pm.moveState == MoveState.Idle && wc.preset) || wc.shootState;
            animator.SetBool("IkEnable", ikEnable);

            // apply hand ik with smooth lerp
            var _aimRigWeight = ikEnable ? 1 : 0;
            aimRig.weight = Mathf.Lerp(aimRig.weight, _aimRigWeight, Time.deltaTime * speedChangeRate);

            // change hand ik position between hipfire and aiming
            var _handTargetPos = !wc.aiming ? rightHandTargetPos : rightHandTargetAim;
            rightHandTarget.localPosition = Vector3.Lerp(rightHandTarget.localPosition, _handTargetPos, Time.deltaTime * speedChangeRate);
            leftHandTarget.localPosition = Vector3.Lerp(leftHandTarget.localPosition, leftHandTargetPos, Time.deltaTime * speedChangeRate);

            if (characterManager == null) return;

            //sync position and rotation
            transform.position = characterManager.transform.position;
            transform.rotation = characterManager.transform.rotation;

            aimTarget.position = characterManager.aimTarget.position;
        }
        else
        {
            // enable ik if aiming,idle or shooting
            bool ikEnable = animator.GetBool("IkEnable");

            // apply hand ik with smooth lerp
            var _aimRigWeight = ikEnable ? 1 : 0;
            aimRig.weight = Mathf.Lerp(aimRig.weight, _aimRigWeight, Time.deltaTime * speedChangeRate);
        }

    }

    private void JumpManagement()
    {
        networkAnimator.SetTrigger("Jump");
    }

    private void FireWeapon()
    {
        networkAnimator.SetTrigger("Fire");
        ServerRequestFire();
    }

    private void DropWeapon()
    {
        ServerRequestDrop();
    }

    private void EquipWeapon(WeaponData wd)
    {
        networkAnimator.SetTrigger("ChangeWeapon");
        ServerRequestEquip(wd.gunID);
    }

    [ServerRpc] private void ServerRequestEquip(int id) => ClientEquipWeapon(id);
    [ServerRpc] private void ServerRequestDrop() => ClientDropWeapon();
    [ServerRpc] private void ServerRequestFire() => ClientFireWeapon();

    [ObserversRpc]
    private void ClientEquipWeapon(int id)
    {
        WeaponData weapon = weaponPrefabs.GetWeaponData(id);

        //reset current weapon
        foreach (Transform child in handIk)
            Destroy(child.gameObject);
        //create weapon in third person
        var gun = Instantiate(weapon.gunModel, handIk);
        //setup weapon hand ik
        gun.transform.localPosition = weapon.thirdPersonData.ikHandPos;
        rightHandTargetPos = weapon.thirdPersonData.rightHandTarget;
        rightHandTargetAim = weapon.thirdPersonData.rightHandTargetAim;
        leftHandTargetPos = weapon.thirdPersonData.leftHandTarget;

        GunSetup(gun.transform, weapon.bulletCasting);

        if (IsOwner)
        {
            foreach (var mesh in gun.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }
        }
    }

    [ObserversRpc]
    private void ClientDropWeapon()
    {
        foreach (Transform child in handIk)
            Destroy(child.gameObject);
    }

    [ObserversRpc(ExcludeOwner =true)]
    private void ClientFireWeapon()
    {
        gunAnim.Play("Fire");
        muzzleVFX.Play();
        Instantiate(bulletCasting, castPoint.position, castPoint.rotation, prefabHolder);
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

    private void GunSetup(Transform gun,GameObject casting)
    {
        gunAnim = gun.GetComponent<Animator>();
        muzzleVFX = gun.Find("muzzle").GetComponentInChildren<ParticleSystem>();
        castPoint = gun.Find("castPoint");
        bulletCasting = casting;
    }
}
