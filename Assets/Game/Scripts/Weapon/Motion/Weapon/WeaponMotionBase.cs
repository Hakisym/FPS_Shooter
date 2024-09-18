using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMotionBase : MonoBehaviour
{
    protected IWeaponStateProvider stateProvider;
    protected Vector3 originalPosition;
    protected Quaternion originalRotation;
    public Transform target;

    public virtual void Setup(IWeaponStateProvider provider)
    {
        this.stateProvider = provider;
        originalPosition = target.localPosition;
        originalRotation = target.localRotation;
    }
}
