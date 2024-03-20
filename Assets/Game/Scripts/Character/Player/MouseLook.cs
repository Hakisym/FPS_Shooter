using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public float wantedCameraXRotation;
    public float wantedYRotation;
    public float yRotationSpeed;
    public float xCameraSpeed;
    public Transform playerBody;

    private float currentCameraXRotation;
    private float currentYRotation;
    private float rotationYVelocity;
    private float cameraXVelocity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        wantedYRotation += mouseX;
        wantedCameraXRotation -= mouseY;
        // Clamp pitch
        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, -90f, 90f);

        //smooth rotate to current rotation
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
        currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

        // Rotate player and camera
        transform.localRotation = Quaternion.Euler(currentCameraXRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
        //playerBody.Rotate(Vector3.up * mouseX);
    }
}
