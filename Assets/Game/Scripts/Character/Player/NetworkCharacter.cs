using FishNet.Connection;
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
    public Camera playerCam;
    public IDamageable damageable;
    public Inventory inventory;
    public CameraManager cameraManager;
    public Transform aimTarget;

    //network raycast
    [SerializeField] LayerMask weaponHitLayers;


    private void OnEnable()
    {
        weaponController.OnRaycastShoot = ShootRPCSender;
    }

    private void ShootRPCSender(float damage, Vector3 position, Vector3 direction, float range)
    {
        ServerShootRPC(damage, position, direction, range);
    }

    [ServerRpc]
    private void ServerShootRPC(float damage, Vector3 origin, Vector3 dir, float range, NetworkConnection conn = null)
    {
        //Will be set to true again if bullet penetrated through something
        bool continueLoop = true;

        RaycastHit hit;
        Vector3 pos = origin;

        float remainingDistance = range;

        int tries = 0;

        while (tries < 10 && Physics.Raycast(pos, dir, out hit, remainingDistance, weaponHitLayers))
        {
            Debug.DrawRay(pos, dir * hit.distance, Color.red, 1f);
            //Add
            tries++;
            //Distract distance
            remainingDistance -= hit.distance;
            // Set new position slightly forward from the hit point to avoid hitting the same spot
            pos = hit.point + dir * 0.01f;
            if (continueLoop)
            {
                #region Damage
                //Check if we hit a player
                if (hit.collider.GetComponentInParent<PlayerHealth>())
                {
                    //Check if we hit ourselves
                    if (hit.transform.root.GetComponent<NetworkObject>().OwnerId == conn.ClientId) continue;
                    //Set Loop to false
                    continueLoop = false;
                    hit.collider.GetComponentInParent<PlayerHealth>().health.Value -= damage;
                }
                #endregion
            }
        }
    }
}
