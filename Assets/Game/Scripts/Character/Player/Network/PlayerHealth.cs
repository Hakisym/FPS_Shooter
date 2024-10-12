using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerHealth : NetworkBehaviour
{
    public readonly SyncVar<float> health = new SyncVar<float>(200);
    private HealthDisplay healthDisplay;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!base.IsOwner) return;
        healthDisplay = GetComponentInChildren<PlayerHUD>().healthDisplay;
    }
    private void Update()
    {
        if (!base.IsOwner) return;

        healthDisplay.SetHealthValue(health.Value);
    }

    public void ServerApplyDamage(float dmg)
    {
        //Check for hitpoints
        if (health.Value > 0)
        {
            //Apply damage
            health.Value -= dmg;
            //Tell HUD
            DisplayShot(Owner);
            //Check for death
            if (health.Value <= 0) Die();
        }
    }
    
    [ServerRpc]
    public void NetworkApplyDamage(float dmg)
    {
        ServerApplyDamage(dmg);
    }

    private void Die()
    {
        NotifyDeath(LocalConnection);
        GetComponentInParent<ActorManager>().Respawn();
        base.Despawn();
    }

    [TargetRpc] private void DisplayShot(NetworkConnection conn) => UIManager.instance.damageIndicator.Show();
    [TargetRpc] private void NotifyDeath(NetworkConnection conn)
    {
        GameManager.instance.DeathCamera.Enable(transform);
    }
}
