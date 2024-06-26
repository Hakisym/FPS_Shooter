using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorManager : MonoBehaviour
{
    public Actor actor { get; set; }

    public bool respawnable;
    public bool spawnOnAwake;
    public float spawnDelay;
    public Actor actorPrefab;

    public bool randomizeName;
    public string randomNamePrefix;

    public int kills;
    public int deaths;

    public string ActorName { get; set; }

    private void Start()
    {
        if (!actorPrefab) actorPrefab = GetComponentInChildren<Actor>();

        if (randomizeName)
        {
            ActorName = randomNamePrefix + Random.Range(0, 1000);
        }

        if (spawnOnAwake)
        {
            foreach (Transform t in transform) t.gameObject.SetActive(false);
            SpawnActor();
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
