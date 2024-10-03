using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public float xClampValue;

    private bool canLook;
    [HideInInspector] public float xRotation;
    [HideInInspector] public float yRotation;
    private CameraRecoil cameraRecoil;

    [HideInInspector] public float cameraRotationX;
    [HideInInspector] public float cameraRotationY;

    private void Awake()
    {
        cameraRecoil = GetComponentInChildren<CameraRecoil>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        canLook = true;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.canLook = !paused;
    }

    private void Update()
    {
        if (!canLook) return;
        LookRotate();
    }

    private void LookRotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yRotation += mouseX;
        xRotation -= mouseY;

        // Periodically reset yRotation to keep the value within reasonable bounds
        if (yRotation > 360f || yRotation < -360f)
        {
            yRotation = 0f;
        }

        //// Periodically reset xRotation to keep the value within reasonable bounds
        //if (xRotation > 360f || xRotation < -360f)
        //{
        //    xRotation = 0f;
        //    cameraRecoil.resultCameraRotation.x += 360f;
        //    cameraRecoil.currentRotation.x += 360f;
        //}

        cameraRotationX = xRotation + cameraRecoil.resultCameraRotation.x;
        cameraRotationY = yRotation + cameraRecoil.resultCameraRotation.y;

        // Clamp pitch
        if (cameraRotationX < -xClampValue || cameraRotationX > xClampValue)
        {
            yRotation -= mouseX;
            xRotation += mouseY;
            return;
        }

        Vector3 cameraRotation = new Vector3(cameraRotationX, 0, 0);
        Vector3 playerRotation = new Vector3(0, cameraRotationY, 0);

        // Rotate player and camera
        transform.localRotation = Quaternion.Euler(cameraRotation);
        playerBody.localRotation = Quaternion.Euler(playerRotation);
    }
}
