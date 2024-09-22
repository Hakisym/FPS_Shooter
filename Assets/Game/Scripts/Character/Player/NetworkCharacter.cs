using FishNet.Object;
using System;
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

    //network raycast
    [SerializeField] LayerMask playerMask;


    private void OnEnable()
    {
        weaponController.OnRaycastShoot += ShootRPCSender;
    }

    private void ShootRPCSender(float damage, Ray ray)
    {
        ServerShootRPC(damage, ray);
    }

    [ServerRpc]
    private void ServerShootRPC(float damage, Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, playerMask))
        {
            if (hit.transform.root.GetComponent<PlayerHealth>())
            {
                hit.transform.root.GetComponent<PlayerHealth>().health.Value -= damage;
            }
        }
}
}
