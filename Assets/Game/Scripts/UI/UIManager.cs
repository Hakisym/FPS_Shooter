using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public bool HitMarker;
    public bool HitMarkerSFX;
    public bool KillMarker;
    public bool KillMarkerSFX;

    [HideInInspector] public InteractHUD InteractUI;
    [HideInInspector] public PauseMenu pauseMenu;
    private HitMarker hitMarker;
    private KillFeed killFeed;
    private DamageIndicator damageIndicator;

    public static UIManager instance;

    public Action<bool,bool> OnEnemyHit;
    public Action<Actor,Actor> OnEnemyKill;
    public Action OnReceiveDamage;

    private void Awake()
    {
        instance = this;
        InteractUI = GetComponentInChildren<InteractHUD>();
        pauseMenu = GetComponentInChildren<PauseMenu>();
        hitMarker = GetComponentInChildren<HitMarker>();
        killFeed = GetComponentInChildren<KillFeed>();
        damageIndicator = GetComponentInChildren<DamageIndicator>();

        InteractUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        OnEnemyHit += HandleHitMarker;
        OnEnemyKill += HandleKillFeed;
        OnReceiveDamage += HandleDamageIndicator;
    }

    private void OnDisable()
    {
        OnEnemyHit -= HandleHitMarker;
        OnEnemyKill -= HandleKillFeed;
        OnReceiveDamage -= HandleDamageIndicator;
    }

    private void Update()
    {
        hitMarker.enableKillMarker = KillMarker;
    }

    private void HandleHitMarker(bool isKill,bool showMarker)
    {
        hitMarker.Show(HitMarker && showMarker, HitMarkerSFX, isKill);
    }

    private void HandleKillFeed(Actor killer,Actor victim)
    {
        killFeed.Show(killer,victim);
        hitMarker.Show(HitMarker, HitMarkerSFX, true);
    }

    private void HandleDamageIndicator()
    {
        damageIndicator.Show();
    }
}
