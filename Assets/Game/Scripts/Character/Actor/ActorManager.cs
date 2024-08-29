using FishNet.Connection;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Random = UnityEngine.Random;

public class ActorManager : NetworkBehaviour
{
    public Actor actor { get; set; }
    public GameObject networkPlayer { get; set; }
    public CharacterManager characterManager { get; set; }

    public bool respawnable;
    public bool spawnOnAwake;
    public float spawnDelay;
    public Actor actorPrefab;
    [SerializeField] GameObject playerPrefab;

    public bool randomizeName;
    public string randomNamePrefix;

    public int kills;
    public int deaths;

    public string ActorName { get; set; }

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();

        if (!actorPrefab) actorPrefab = GetComponentInChildren<Actor>();

        if (randomizeName)
        {
            ActorName = randomNamePrefix + Random.Range(0, 1000);
        }
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        if (base.Owner.IsLocalClient)
        {
            SpawnActor();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (playerPrefab)
        {
            networkPlayer = Instantiate(playerPrefab);
            ServerManager.Spawn(networkPlayer, base.Owner);
        }
    }

    private void DestroyPlayerPrefab()
    {
        //disable controller
        if (networkPlayer)
        {
            Destroy(networkPlayer);
            //disable rig ik
            networkPlayer.GetComponent<RigBuilder>().enabled = false;
            //destroy third person actor
            Destroy(networkPlayer.gameObject, 3f);
        }
    }

    public void Respawn()
    {
        Invoke(nameof(SpawnActor), spawnDelay);
    }

    private void SpawnActor()
    {
        actor = Instantiate(actorPrefab,transform);
        actor.actorName = ActorName;
        actor.gameObject.SetActive(true);
    }
}
