using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovManager : MonoBehaviour
{
    [Header("FovStats")]
    public float cameraFovSprinting = 70f;
    public float cameraFovAiming = 50f;
    public float cameraFovSmoothing = 5f;
    [Space]
    public PlayerMovement pm;
    public WeaponController wc;

    private Camera mainCamera;
    private Camera weaponCamera;
    private float originalFov;
    private float currentFov;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        weaponCamera = transform.Find("WeaponCamera").GetComponent<Camera>();
        originalFov = mainCamera.fieldOfView;
    }

    void Update()
    {
        if (pm.isSprinting)
            currentFov = cameraFovSprinting;
        else if (wc.aiming)
            currentFov = cameraFovAiming;
        else
            currentFov = originalFov;

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentFov, Time.deltaTime * cameraFovSmoothing);
        weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, currentFov, Time.deltaTime * cameraFovSmoothing);
    }
}
