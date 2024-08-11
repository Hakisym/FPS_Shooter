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
    private float yRotation = 0f;
    public CameraRecoil cameraRecoil;
    public WeaponController weaponController;

    private Vector3 lastRotation;
    private float cameraRotationX;
    private float cameraRotationY;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controllable = true;
        UIManager.instance.pauseMenu.OnPausedStateChanged += (bool paused) => this.controllable = !paused;

        mainCamera = Camera.main;

        weaponController.OnStartShoot += () => {
            lastRotation = new Vector3(cameraRotationX, cameraRotationY, 0);
        };
        weaponController.OnEndShoot += () => {
            Vector3 currentRotation = new Vector3(cameraRotationX, cameraRotationY, 0);
            Vector3 rot = currentRotation - lastRotation;

            if (rot.x > 0) cameraRecoil.canReturn = false;
            else
            {
                cameraRecoil.canReturn = true;
                cameraRecoil.returnValue = cameraRecoil.currentRotation - rot;
            }   
        };
    }

    private void Update()
    {
        if (!controllable) return;
        LookRotate();

        //aim raycast
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, aimMask))
        {
            aimTarget.position = hitInfo.point;
        }
        else
        {
            aimTarget.position = ray.GetPoint(50);
        }
    }

    private void LookRotate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity + addedLookValue.y;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity + addedLookValue.x;

        addedLookValue = Vector2.zero;

        yRotation += mouseX;
        xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraRotationX = xRotation + cameraRecoil.resultCameraRotation.x;
        cameraRotationY = yRotation + cameraRecoil.resultCameraRotation.y;

        // Clamp pitch
        if (cameraRotationX < -90f || cameraRotationX > 90f)
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

    public void AddLookValue(Vector2 value)
    {
        addedLookValue += value;
    }
}
