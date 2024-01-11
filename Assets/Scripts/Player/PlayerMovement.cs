using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    [Space]
    [Header("Move Stats")]
    public float walkSpeed = 8f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 4f;
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
    [Space]
    [Header("Crouch")]
    public Transform cameraHolder;
    public float crouchHeight;
    [Space]
    [Header("Sprint")]
    public float cameraFovSprinting = 70f;
    public float cameraFovSmoothing = 5f;
    [Space]
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip runSound;
    [Space]
    [Header("Reference")]
    public PlayerBehaviour pb;
    public Animator animator;
    public WeaponSystem ws;

    private Vector3 moveDirection;
    private float originalHeight;
    private float speed;
    private float mouseX;
    private float mouseY;

    private void Start()
    {
        originalHeight = controller.height;
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

        Jump();

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
        // Move input
        mouseX = Input.GetAxis("Horizontal");
        mouseY = Input.GetAxis("Vertical");
        moveDirection = transform.right * mouseX + transform.forward * mouseY;

        // Sprint input
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && moveDirection.sqrMagnitude > 0 && !pb.isAiming && !ws.shooting)
        {
            isSprinting = !isSprinting;
        }

        // Crouch input
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && !isSprinting)
        {
            isCrouching = !isCrouching;
        }
    }

    void Crouch()
    {
        if (isSprinting || !isGrounded)
        {
            isCrouching = false;
        }

        // Camera crouch
        Vector3 finalPosition = (controller.height * 0.75f - originalHeight / 2f) * Vector3.up;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, finalPosition, Time.deltaTime * 10f);

        // Character collider crouch
        controller.height = isCrouching ? crouchHeight : originalHeight;
        controller.center = (controller.height - originalHeight) / 2f * Vector3.up;
    }

    private void Sprint()
    {
        if (moveDirection.sqrMagnitude <= 0 || pb.isAiming || ws.shooting)
        {
            isSprinting = false;
        }

        // Camera fov
        float cameraFov = isSprinting ? cameraFovSprinting : 60;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, cameraFov, Time.deltaTime * 5f);

        if (isSprinting)
        {
            animator.SetBool("sprinting", true);
        }
        else
        {
            animator.SetBool("sprinting", false);
        }
    }

    private void HandleVelocity()
    {
        // Check velocity
        if (isSprinting)
        {
            speed = sprintSpeed;
        }
        else if (isCrouching || pb.isAiming)
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
        // Clamp movement
        Vector3 movement = Vector3.ClampMagnitude(moveDirection, 1f);

        // Move
        controller.Move(movement * speed * Time.deltaTime);
    }

    private void Jump()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
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
