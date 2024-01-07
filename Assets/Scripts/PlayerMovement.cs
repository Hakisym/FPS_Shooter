using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    [Space]
    public float walkSpeed = 8f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    [Space]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    [Space]
    public Vector3 velocity;
    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;
    [Space]
    public Transform cameraHolder;
    public float crouchHeight;
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset gravity
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        CheckInput();

        Crouch();
        
        // Check velocity
        if (isSprinting)
        {
            speed = sprintSpeed;
        }
        else if (isCrouching)
        {
            speed = crouchSpeed;
        }
        else
        {
            speed = walkSpeed;
        }

        // Clamp movement
        Vector3 movement = transform.right * mouseX + transform.forward * mouseY;
        movement = Vector3.ClampMagnitude(movement, 1f);

        // Move
        controller.Move(movement * speed * Time.deltaTime);

        // Jump
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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

        // Sprint input
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isCrouching)
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
        // Camera crouch
        Vector3 finalPosition = (controller.height * 0.75f - originalHeight/2f) * Vector3.up;
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, finalPosition, Time.deltaTime * 10f);

        // Character collider crouch
        controller.height = isCrouching ? crouchHeight : originalHeight;
        controller.center = (controller.height - originalHeight) / 2f * Vector3.up;
    }
}
