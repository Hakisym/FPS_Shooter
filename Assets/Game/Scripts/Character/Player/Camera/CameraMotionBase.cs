using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraMotionBase : MonoBehaviour
{
    public Vector3 targetPosition { get; set; }
    public Vector3 targetRotation { get; set; }
}
