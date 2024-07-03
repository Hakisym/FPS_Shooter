using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public bool HitMarker;
    public bool HitMarkerSFX;
    public bool KillFeed;

    public RectTransform crosshair;

    [HideInInspector] public InteractHUD InteractUI;
    [HideInInspector] public PauseMenu pauseMenu;
    private HitMarker hitMarker;
    private KillFeed killFeed;
    private DamageIndicator damageIndicator;

    public static UIManager instance;

    public Action<bool,Vector3> OnEnemyHit;
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
        if (Input.GetMouseButtonDown(2))
        {
            crosshair.gameObject.SetActive(!crosshair.gameObject.activeSelf);
        }
    }

    private void HandleHitMarker(bool critical,Vector3 position)
    {
        hitMarker.Show(HitMarker, HitMarkerSFX, critical,position);
    }

    private void HandleKillFeed(Actor killer,Actor victim)
    {
        if (KillFeed) killFeed.Show(killer, victim);
    }

    private void HandleDamageIndicator()
    {
        damageIndicator.Show();
    }
}
