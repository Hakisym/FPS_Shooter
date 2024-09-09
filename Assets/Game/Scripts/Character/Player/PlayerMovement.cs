using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    [Space]
    [Header("Move Stats")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 6f;
    public float crouchSpeed = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float stepIntervalWalk = 5;
    public float stepIntervalSprint = 4;
    public float stepIntervalCrouch = 3;
    [Space]
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [Space]
    [Header("Move State")]
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isCrouching;
    [HideInInspector] public bool canMove;
    [Space]
    [Header("Crouch")]
    public Transform cameraHolder;
    public float crouchHeight;
    [Space]
    [Header("Sounds")]
    public AudioProfile footstepWalk;
    public AudioProfile footstepSprint;
    public AudioProfile footstepCrouch;
    [Space]
    [Header("Reference")]
    public Animator animator;
    public WeaponController wc;

    public UnityEvent OnJump;
    public UnityEvent OnLand;
    public UnityEvent OnSprint;
    private bool previouslyGrounded;

    public float currentSpeed { get => speed; }
    public float moveVel { get => moveDirection.sqrMagnitude; }

    private Vector3 moveDirection;
    private float originalHeight;
    private float speed;
    private float mouseX;
    private float mouseY;

    private Audio footstepsAudio = new Audio();
    private float stepCycle;
    private float nextStep;

    private void Awake()
    {
        footstepsAudio.Equip(gameObject, null);
    }

    private void Start()
    {
        originalHeight = controller.height;
        canMove = true;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.canMove = !paused;
    }

    private void Update()
    {
        CheckGround();

        HandleGravity();

        CheckInput();

        Crouch();

        Sprint();

        HandleVelocity();

        Move();

        HandleMotionEvent();
    }

    private void FixedUpdate()
    {
        ProgressStepCycle();
    }

    private void CheckGround()
    {
        // Chech ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    private void HandleGravity()
    {
        // Reset gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void CheckInput()
    {
        if (!canMove)
        {
            moveDirection = Vector3.zero;
            return;
        }

        // Move input
        mouseX = Input.GetAxis("Horizontal");
        mouseY = Input.GetAxis("Vertical");
        moveDirection = transform.right * mouseX + transform.forward * mouseY;
        // Clamp movement
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Sprint input
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && mouseY > 0)
        {
            if (!isSprinting) OnSprint?.Invoke();
            isSprinting = !isSprinting;
        }

        // Crouch input
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isSprinting)
            isCrouching = !isCrouching;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded) Jump();
    }

    void Crouch()
    {
        if (isSprinting || !isGrounded)
        {
            isCrouching = false;
        }

        // Camera crouch
        Vector3 finalPosition = (controller.height * 0.85f) * Vector3.up;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, finalPosition, Time.deltaTime * 10f);

        // Character collider crouch
        controller.height = isCrouching ? crouchHeight : originalHeight;
        controller.center = (controller.height / 2) * Vector3.up;
    }

    private void Sprint()
    {
        if (Input.GetAxisRaw("Vertical") < 1 || wc.shootState || Input.GetMouseButtonDown(1)) isSprinting = false;
    }

    private void HandleVelocity()
    {
        // Check velocity
        if (isSprinting)
        {
            speed = sprintSpeed;
        }
        else if (isCrouching || wc.aiming)
        {
            speed = crouchSpeed;
        }
        else
        {
            speed = walkSpeed;
        }
    }

    private void Move()
    {
        // Move
        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void HandleMotionEvent()
    {
        if (previouslyGrounded && !isGrounded) OnJump?.Invoke();
        if (!previouslyGrounded && isGrounded) OnLand?.Invoke();

        previouslyGrounded = isGrounded;
    }

    private void ProgressStepCycle()
    {
        //stop if not grounded
        if (!isGrounded || !canMove) return;

        AudioProfile footstepSFX;
        float stepInterval;

        //change step interval
        if (isSprinting)
        {
            footstepSFX = footstepSprint;
            stepInterval = stepIntervalSprint;
        }
        else if (isCrouching || wc.aiming)
        {
            footstepSFX = footstepCrouch;
            stepInterval = stepIntervalCrouch;
        }
        else
        {
            footstepSFX = footstepWalk;
            stepInterval = stepIntervalWalk;
        }

        //check if taking input
        if (velocity.sqrMagnitude > 0 && (Input.GetAxisRaw("Horizontal") != 0) || Input.GetAxisRaw("Vertical") != 0)
        {
            //update step cycle
            stepCycle += 10 * Time.fixedDeltaTime;
        }

        //check step cycle not equal to next step in order to update right
        if (!(stepCycle > nextStep))
        {
            return;
        }

        nextStep = stepCycle + stepInterval;

        footstepsAudio.Update(footstepSFX);
        footstepsAudio.PlayOneShot(footstepSFX);
    }
}
