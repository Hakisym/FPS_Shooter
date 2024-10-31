using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

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
        DisplayRespawnTime(LocalConnection, spawnDelay);
    }

    [TargetRpc]
    private void DisplayRespawnTime(NetworkConnection conn, float time) => UIManager.instance.respawnPanel.StartRespawnTime(time);

    private void Spawn()
    {
        actor = Instantiate(actorPrefab,transform);
        base.Spawn(actor, Owner);
    }
}
