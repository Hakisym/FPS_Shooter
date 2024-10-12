using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

public class ThirdPersonController : NetworkBehaviour
{
    //reference
    public WeaponDataCollection weaponPrefabs;
    private Animator animator;
    private NetworkAnimator networkAnimator;
    [SerializeField] NetworkCharacter characterManager;
    private PlayerMovement pm;
    private WeaponController wc;
    private Camera playerCam;
    private SkinnedMeshRenderer meshRenderer;
    private Ragdoll ragdoll;

    private GameObject bulletCasting;
    private Transform prefabHolder;
    private Animator gunAnim;
    private ParticleSystem muzzleVFX;
    private Transform castPoint;

    public Transform weaponHolder;

    public float speedChangeRate;
    public float fallTimeout;

    private Vector2 targetMove;
    private float _animationBlend;
    private float _fallTimeoutDelta;

    public bool showLocalPlayerModel;
    //IK
    public float armsRotation = 8f;
    public float minVerticalAngle = -60;
    public float maxVerticalAngle = 70;
    private Transform hips, spine, chest;                          // Avatar bone transforms.
    private Vector3 initialRootRotation;                           // Initial root bone local rotation.
    private Vector3 initialHipsRotation;                           // Initial hips rotation related to the root bone.
    private Vector3 initialSpineRotation;                          // Initial spine rotation related to the hips bone.
    private Vector3 initialChestRotation;                          // Initial chest rotation related to the spine bone.
    private Vector3 castRelativeOrigin;
    private float distToHand;

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (base.Owner.IsLocalClient)
        {
            Initialize();
            if (!showLocalPlayerModel)
                meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();
        ragdoll = GetComponent<Ragdoll>();

        Setup(characterManager);
        prefabHolder = FindObjectOfType<PoolManager>().poolHolder;

        hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
        Transform root = hips.parent;

        // Get initial values.
        initialRootRotation = (root == transform) ? Vector3.zero : root.localEulerAngles;
        initialHipsRotation = hips.localEulerAngles;
        initialSpineRotation = spine.localEulerAngles;
        initialChestRotation = chest.localEulerAngles;

        Transform neck = animator.GetBoneTransform(HumanBodyBones.Neck);
        castRelativeOrigin = neck.position - this.transform.position;
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
        distToHand = (rightHand.position - neck.position).magnitude * 1.5f;
    }

    public void Setup(NetworkCharacter _characterManager)
    {
        characterManager = _characterManager;
        pm = _characterManager.playerMovement;
        wc = _characterManager.weaponController;
        playerCam = _characterManager.playerCam;

        //setup ragdoll
        //HealthSystem healthSystem = (HealthSystem)characterManager.damageable;
        //healthSystem.ragdoll = this.ragdoll;
    }

    private void Initialize()
    {
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

            animator.SetInteger("Weapon", (wc.preset) ? wc.preset.type == WeaponType.Long ? 1 : 2 : 0);
            animator.SetBool("Aim", wc.aiming);
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
        foreach (Transform child in weaponHolder)
            Destroy(child.gameObject);
        //create weapon in third person
        var gun = Instantiate(weapon.gunModel, weaponHolder);

        GunSetup(gun.transform, weapon.bulletCasting);

        if (IsOwner && !showLocalPlayerModel)
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
        foreach (Transform child in weaponHolder)
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

    public void OnAnimatorIK(int layerIndex)
    {
        Debug.DrawRay(this.transform.position + castRelativeOrigin, playerCam.transform.forward * distToHand, Color.red);

        // Orientate upper body where camera  is targeting.
        Quaternion targetRot = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        targetRot *= Quaternion.Euler(initialRootRotation);
        targetRot *= Quaternion.Euler(initialHipsRotation);
        targetRot *= Quaternion.Euler(initialSpineRotation);
        // Set upper body horizontal orientation.
        animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Inverse(hips.rotation) * targetRot);

        // Keep upper body orientation regardless strafe direction.
        float xCamRot = Quaternion.LookRotation(playerCam.transform.forward).eulerAngles.x;

        // Convert angle to the range of -180 to 180 degrees to handle the wrap-around issue
        if (xCamRot > 180f) xCamRot -= 360f;
        // Clamp it
        xCamRot = Mathf.Clamp(xCamRot, minVerticalAngle, maxVerticalAngle);
        // Convert it back to the 0-360 range:
        if (xCamRot < 0f) xCamRot += 360f;

        targetRot = Quaternion.AngleAxis(xCamRot + armsRotation, this.transform.right);

        if (wc.preset && wc.preset.type == WeaponType.Long && pm.moveState != MoveState.Sprinting)
        {
            // Correction for long weapons.
            targetRot *= Quaternion.AngleAxis(9f, this.transform.right);
            targetRot *= Quaternion.AngleAxis(20f, this.transform.up);
        }

        targetRot *= spine.rotation;
        targetRot *= Quaternion.Euler(initialChestRotation);
        // Set upper body vertical orientation.
        animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Inverse(spine.rotation) * targetRot);
    }
}
