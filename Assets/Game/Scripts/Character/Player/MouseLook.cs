using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    public Transform aimTarget;
    public LayerMask aimMask;
    private Camera mainCamera;

    private bool controllable;
    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controllable = true;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.controllable = !paused;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (!controllable) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        // Clamp pitch
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate player and camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

        RaycastHit hitInfo;

        if (Physics.Raycast(ray,out hitInfo, Mathf.Infinity,aimMask))
        {
            aimTarget.position = hitInfo.point;
        }
        else
        {
            aimTarget.position = ray.GetPoint(50);
        }
    }
}
