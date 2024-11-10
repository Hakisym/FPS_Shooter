using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public float xClampValue;

    private bool canLook;
    public float xRotation;
    [HideInInspector] public float yRotation;
    private CameraRecoilProcedural cameraRecoil;
    public float xInput;

    private void Awake()
    {
        cameraRecoil = GetComponentInChildren<CameraRecoilProcedural>();
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

        xInput -= mouseY;
        xRotation -= mouseY;
        yRotation += mouseX;

        ResetAngularFloat(ref xInput);
        ResetAngularFloat(ref yRotation);

        xRotation += cameraRecoil.cameraRecoilX;
        yRotation += cameraRecoil.cameraRecoilY;

        // Clamp pitch
        xRotation = Mathf.Clamp(xRotation, -xClampValue, xClampValue);

        Vector3 cameraRotation = new Vector3(xRotation, 0, 0);
        Vector3 playerRotation = new Vector3(0, yRotation, 0);

        // Rotate player and camera
        transform.localRotation = Quaternion.Euler(cameraRotation);
        playerBody.localRotation = Quaternion.Euler(playerRotation);
    }

    // Periodically reset yRotation to keep the value within reasonable bounds
    private void ResetAngularFloat(ref float value)
    {
        if (value > 360f || value < -360f)
        {
            value = 0f;
        }
    }
}
