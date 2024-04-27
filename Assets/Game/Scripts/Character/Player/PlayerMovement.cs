using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    [Space]
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [Space]
    [Header("Move State")]
    public Vector3 velocity;
    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;
    public bool canMove;
    [Space]
    [Header("Crouch")]
    public Transform cameraHolder;
    public float crouchHeight;
    [Space]
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip runSound;
    [Space]
    [Header("Reference")]
    public Animator animator;
    public WeaponController wc;

    public UnityEvent OnJump;
    public UnityEvent OnLand;

    public float currentSpeed { get => speed; }
    public float moveVel { get => moveDirection.sqrMagnitude; }
    private Vector3 moveDirection;
    private float originalHeight;
    private float speed;
    private float mouseX;
    private float mouseY;

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

        HandleSFX();
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
            isSprinting = !isSprinting;

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
        OnJump?.Invoke();
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void HandleSFX()
    {
        // Play foot sound
        if (isGrounded && moveDirection.sqrMagnitude > 0)
        {
            audioSource.clip = isSprinting ? runSound : walkSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Pause();
        }
    }
}
