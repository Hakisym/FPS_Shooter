using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    protected WeaponController wc;
    protected PlayerLook pl;
    public Vector3 resultCameraRotation;
    public Vector3 currentRotation;
    protected WeaponData currentPreset;

    public virtual void Awake()
    {
        wc = GetComponentInParent<WeaponController>();
        pl = GetComponentInParent<PlayerLook>();
    }

    public virtual void OnEnable()
    {
        wc.OnWeaponChange += (WeaponData preset) => { currentPreset = preset; };
    }

    public virtual void ApplyRecoil(bool aiming) { }
}
