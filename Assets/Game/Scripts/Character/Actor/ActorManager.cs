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
    public GameObject actor;

    public float spawnDelay;
    public GameObject actorPrefab;

    public int kills;
    public int deaths;

    public void Respawn()
    {
        Invoke(nameof(Spawn), spawnDelay);
    }

    private void Spawn()
    {
        actor = Instantiate(actorPrefab,transform);
        base.Spawn(actor, Owner);
    }
}
