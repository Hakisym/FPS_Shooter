using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
}
