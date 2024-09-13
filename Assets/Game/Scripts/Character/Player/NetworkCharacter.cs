using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCharacter : NetworkBehaviour
{
    public Actor actor;
    public WeaponController weaponController;
    public PlayerMovement playerMovement;
    public IDamageable damageable;
    public Inventory inventory;
    public CameraManager cameraManager;
    public Transform aimTarget;

}
