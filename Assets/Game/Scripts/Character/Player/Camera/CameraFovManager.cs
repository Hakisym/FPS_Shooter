using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovManager : MonoBehaviour
{
    [Header("FovStats")]
    public float sprintingFOV = 10f;
    public float sprintingFOVOverlay = 10f;
    public float aimingFOV = -10f;
    public float aimingFOVOverlay = -10f;
    public float cameraFovSmoothing = 5f;
    [Space]
    public PlayerMovement pm;
    public WeaponController wc;

    private Camera mainCamera;
    private Camera weaponCamera;
    private float originalMainFov;
    private float originalWeaponFov;
    private float currentMainFov;
    private float currentWeaponFov;
    private float targetMainFov;
    private float targetWeaponFov;

    private void Awake()
    {
        mainCamera = Camera.main;
        weaponCamera = transform.Find("WeaponCamera").GetComponent<Camera>();
        originalMainFov = mainCamera.fieldOfView;
        originalWeaponFov = weaponCamera.fieldOfView;
    }

    void Update()
    {
        if (pm.isSprinting)
        {
            currentMainFov = originalMainFov + sprintingFOV;
            currentWeaponFov = originalWeaponFov + sprintingFOVOverlay;
        }
        else
        {
            currentMainFov = originalMainFov;
            currentWeaponFov = originalWeaponFov;
        }

        targetMainFov = Mathf.Lerp(targetMainFov, currentMainFov, Time.deltaTime * cameraFovSmoothing);
        targetWeaponFov = Mathf.Lerp(targetWeaponFov, currentWeaponFov, Time.deltaTime * cameraFovSmoothing);
    }

    public void UpdateAimFov(float percentage)
    {
        mainCamera.fieldOfView = Mathf.Lerp(targetMainFov, targetMainFov + aimingFOV, percentage);
        weaponCamera.fieldOfView = Mathf.Lerp(targetWeaponFov, targetWeaponFov + aimingFOVOverlay, percentage);
    }
}
