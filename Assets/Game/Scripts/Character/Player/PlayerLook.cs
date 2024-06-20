using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    public Transform aimTarget;
    public LayerMask aimMask;
    public Vector2 addedLookValue { get; set; }
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

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity + addedLookValue.y;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity + addedLookValue.x;

        addedLookValue = Vector2.zero;

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

    public void AddLookValue(Vector2 value)
    {
        addedLookValue += value;
    }
}
