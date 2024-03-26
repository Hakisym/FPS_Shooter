using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFovManager : MonoBehaviour
{
    [Header("FovStats")]
    public float sprintingFOV = 10f;
    public float aimingFOV = -10f;
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

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        weaponCamera = transform.Find("WeaponCamera").GetComponent<Camera>();
        originalMainFov = mainCamera.fieldOfView;
        originalWeaponFov = weaponCamera.fieldOfView;
    }

    void Update()
    {
        if (pm.isSprinting)
        {
            currentMainFov = originalMainFov + sprintingFOV;
            currentWeaponFov = originalWeaponFov + sprintingFOV;
        }
        else if (wc.aiming)
        {
            currentMainFov = originalMainFov + aimingFOV;
            currentWeaponFov = originalWeaponFov + aimingFOV;
        }
        else
        {
            currentMainFov = originalMainFov;
            currentWeaponFov = originalWeaponFov;
        }

        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentMainFov, Time.deltaTime * cameraFovSmoothing);
        weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, currentWeaponFov, Time.deltaTime * cameraFovSmoothing);
    }
}
