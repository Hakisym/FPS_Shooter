using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponIdentification : MonoBehaviour, IWeaponStateProvider
{
    public WeaponData preset;
    public Transform muzzle;
    public Transform castPoint;
    public Animator animator;
    public WeaponMotionBase[] motions;

    private WeaponController wc;
    private PlayerMovement pm;

    public bool IsAiming => wc.aiming;
    public float AimValue => wc.aimValue;
    public MoveState CurrentMoveState => pm.moveState;
    public WeaponData Preset => preset;

    public bool isVelocityZero => pm.isVelocityZero;

    public Action<bool> OnPlayerLeaveGround { get; set; }

    public void Initialize(WeaponController weaponController, PlayerMovement playerMovement)
    {
        wc = weaponController;
        pm = playerMovement;

        pm.OnJump.AddListener(() => OnPlayerLeaveGround(true));   // Forward the jump event
        pm.OnLand.AddListener(() => OnPlayerLeaveGround(false));   // Forward the land event

        foreach (var motion in motions)
        {
            motion.Setup(this);
        }
    }
}
