using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public List<CameraMotionBase> motions = new List<CameraMotionBase>();

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
        Quaternion rotation = Quaternion.Euler(GetRotation());
        transform.localRotation = rotation;
    }
}
