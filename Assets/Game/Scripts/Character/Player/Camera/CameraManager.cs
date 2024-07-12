using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<CameraMotionBase> motions = new List<CameraMotionBase>();
    public CameraFovManager cameraFovManager;

    [Header("Camera Shake")]
    public CameraShaker cameraShaker;
    public float cameraShakeMagnitude = 1.6f;
    public float cameraShakeRoughness = 7;
    public float cameraShakeFadeInTime = 0.2f;
    public float cameraShakeFadeOutTime = 2f;

    private void Awake()
    {
        motions = GetComponentsInChildren<CameraMotionBase>().ToList();
    }

    private Vector3 GetPosition()
    {
        Vector3 result = Vector3.zero;

        foreach (var motion in motions) result += motion.targetPosition;

        return result;
    }

    private Vector3 GetRotation()
    {
        Vector3 result = Vector3.zero;

        foreach (var motion in motions) result += motion.targetRotation;

        return result;
    }

    private void Update()
    {
        transform.localPosition = GetPosition();
        transform.localRotation = Quaternion.Euler(GetRotation());
    }

    public void ShakeCameras(float multiplier)
    {
        cameraShaker.Shake(cameraShakeMagnitude * multiplier, cameraShakeRoughness, cameraShakeFadeInTime, cameraShakeFadeOutTime);
    }

    public void ShakeCameras(float multiplier, float fadeOutTime)
    {
        cameraShaker.Shake(cameraShakeMagnitude * multiplier, cameraShakeRoughness, cameraShakeFadeInTime, fadeOutTime);
    }

    public void ShakeCameras(float multiplier,float roughness,float fadeOutTime)
    {
        cameraShaker.Shake(cameraShakeMagnitude * multiplier, roughness, cameraShakeFadeInTime, fadeOutTime);
    }
}
