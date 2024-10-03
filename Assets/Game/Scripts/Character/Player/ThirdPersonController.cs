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
    private Transform spine;
    private Transform chest;
    public float armsRotation = 8f;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            characterManager = GetComponentInParent<NetworkCharacter>();
            Setup(characterManager);
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

        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
        chest = animator.GetBoneTransform(HumanBodyBones.Chest);
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

            animator.SetInteger("Weapon", (wc.preset) ? wc.preset.type == WeaponType.Long ? 1 : 2 : 0);
            animator.SetBool("Aim", wc.aiming);

            if (characterManager == null) return;

            //sync position and rotation
            transform.position = characterManager.transform.position;
            transform.rotation = characterManager.transform.rotation;
        }
        else
        {
            
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
        if (wc && wc.preset)
        {
            // Keep upper body orientation regardless strafe direction.
            float xCamRot = Quaternion.LookRotation(playerCam.transform.forward).eulerAngles.x;
            Quaternion targetRot = Quaternion.AngleAxis(xCamRot + armsRotation, this.transform.right);

            if (wc.preset.type == WeaponType.Long)
            {
                // Correction for long weapons.
                targetRot *= Quaternion.AngleAxis(9f, this.transform.right);
                targetRot *= Quaternion.AngleAxis(20f, this.transform.up);
            }

            targetRot *= spine.rotation;
            // Set upper body vertical orientation.
            animator.SetBoneLocalRotation(HumanBodyBones.Chest, Quaternion.Inverse(spine.rotation) * targetRot);
        }
    }
}
