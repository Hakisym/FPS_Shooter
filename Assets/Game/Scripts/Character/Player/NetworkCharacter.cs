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

    //network syncvar
    private readonly SyncVar<int> syncedMoveState = new SyncVar<int>();     // SyncVar to synchronize the move state
    private readonly SyncVar<int> syncedGunId = new SyncVar<int>(-1);       // SyncVar to synchronize the active gun ID

    private void OnEnable()
    {
        weaponController.OnRaycastShoot = ShootRPCSender;
        syncedGunId.OnChange += OnGunChange;
    }

    private void OnGunChange(int prev, int next, bool asServer)
    {
        if (!asServer && !base.IsOwner)
        {
            weaponController.preset = next == -1 ? thirdPerson.weaponPrefabs.GetWeaponData(next) : null;
        }
    }

    private void Update()
    {
        if (base.IsOwner)
        {
            if (weaponController.preset != null)
            {
                if (weaponController.preset.gunID != syncedGunId.Value)
                    SetGunId(weaponController.preset.gunID);
            }
            else
            {
                if (syncedGunId.Value != -1)
                    SetGunId(-1);
            }

            if (((int)playerMovement.moveState) != syncedMoveState.Value)
            {
                SetMoveState(((int)playerMovement.moveState));
            }
        }
        else
        {
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

        while (tries < 10 && Physics.Raycast(pos, dir, out hit, remainingDistance, weaponController.whatIsShootable))
        {
            if (continueLoop) Debug.DrawRay(pos, dir * hit.distance, Color.red, 1f);

            //Add
            tries++;
            //Distract distance
            remainingDistance -= hit.distance;
            // Set new position slightly forward from the hit point to avoid hitting the same spot
            pos = hit.point + dir * 0.01f;
            if (continueLoop)
            {
                //Check if we hit ourselves
                if (hit.transform.root.GetComponent<NetworkObject>()?.OwnerId == conn.ClientId) continue;

                //Set Loop to false
                continueLoop = false;
                #region Damage
                //Check if we hit a player
                if (hit.collider.GetComponentInParent<PlayerHealth>())
                {
                    //Apply local damage
                    hit.collider.GetComponentInParent<PlayerHealth>().ServerApplyDamage(damage);
                    //Since we hit a player, show the hitmarker
                    DisplayHitmarker(conn, hit.point);
                    //Tell other players we hit something
                    WeaponRaycastHit(hit.point, hit.normal, hit.collider.tag);
                }
                #endregion
                else
                {
                    //Tell other players we hit something
                    WeaponRaycastHit(hit.point, hit.normal, hit.collider.tag);
                }
            }
        }
    }

    [TargetRpc] private void DisplayHitmarker(NetworkConnection conn, Vector3 pos) => UIManager.instance.hitMarker.Show(false, pos);

    [ObserversRpc] private void WeaponRaycastHit(Vector3 pos,Vector3 normal,string material)
    {
        if (material == "PlayerCollider")
            Instantiate(weaponController.bloodImpact, pos, Quaternion.LookRotation(normal));
        else
            PoolManager.instance.SpawnGameObject(PoolTag.BulletImpact, pos, Quaternion.LookRotation(normal));
    }

    [ServerRpc] private void SetMoveState(int value) => syncedMoveState.Value = value;
    [ServerRpc] private void SetGunId(int value) => syncedGunId.Value = value;
}
