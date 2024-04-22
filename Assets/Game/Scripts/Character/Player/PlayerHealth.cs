using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable,IControllable
{
    public int maxHealth;
    public int currentHealth;
    private ImpactManager impactManager;

    public Action<bool> OnControllableChanged { get; set; }

    private void Start()
    {
        currentHealth = maxHealth;
        impactManager = GetComponent<ImpactManager>();

        UIManager.instance.pauseMenu.OnPausedStateChanged += HandleControl;
    }

    private void OnDisable() => UIManager.instance.pauseMenu.OnPausedStateChanged -= HandleControl;

    public void TakeDamage(int amount, DamageInfo info)
    {
        UIManager.instance.OnReceiveDamage();

        currentHealth -= amount;

        if(info!=null) impactManager.AddImpact(info.direction, info.force);

        if (currentHealth <= 0)
        {
            Debug.Log("Player is dead!");
        }
    }

    public void HandleControl(bool paused)
    {
        OnControllableChanged?.Invoke(!paused);
    }
}
