using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraMotionBase : MonoBehaviour
{
    public virtual Vector3 targetPosition { get; set; }
    public virtual Vector3 targetRotation { get; set; }
}
