using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponStateProvider
{
    WeaponData Preset { get; }
    bool IsAiming { get; }
    float AimValue { get; }
    MoveState CurrentMoveState { get; }
    bool isVelocityZero { get; }

    event Action OnJump;
    event Action OnLand;
}
