using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
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
    public ThirdPersonController thirdPerson;

    //network raycast
    [SerializeField] LayerMask weaponHitLayers;

    //network syncvar
    private readonly SyncVar<int> syncedMoveState = new SyncVar<int>();     // SyncVar to synchronize the move state
    private readonly SyncVar<int> syncedGunId = new SyncVar<int>(-1);       // SyncVar to synchronize the active gun ID

    private void OnEnable()
    {
        weaponController.OnRaycastShoot = ShootRPCSender;
    }

    private void Update()
    {
        if (base.IsOwner)
        {
            if (weaponController.preset != null && weaponController.preset.gunID != syncedGunId.Value)
            {
                SetGunId(weaponController.preset.gunID);
            }

            if (((int)playerMovement.moveState) != syncedMoveState.Value)
            {
                SetMoveState(((int)playerMovement.moveState));
            }
        }
        else
        {
            if (weaponController.preset?.gunID != syncedGunId.Value && syncedGunId.Value != -1)
            {
                weaponController.preset = thirdPerson.weaponPrefabs.GetWeaponData(syncedGunId.Value);
            }

            if (((int)playerMovement.moveState) != syncedMoveState.Value)
            {
                playerMovement.moveState = (MoveState)syncedMoveState.Value;
            }
        }
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

    [ServerRpc] private void SetMoveState(int value) => syncedMoveState.Value = value;
    [ServerRpc] private void SetGunId(int value) => syncedGunId.Value = value;
}
