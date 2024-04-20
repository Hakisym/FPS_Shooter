using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public bool Crosshair;
    public bool HitMarker;
    public bool HitMarkerSFX;
    public bool KillMarker;
    public bool KillMarkerSFX;

    [HideInInspector] public CrossHair crosshair;
    [HideInInspector] public InteractHUD InteractUI;
    private HitMarker hitMarker;

    public static UIManager instance;

    public Action<bool,bool> OnEnemyHit;
    public Action OnEnemyKill;

    private void Awake()
    {
        instance = this;
        crosshair = GetComponentInChildren<CrossHair>();
        InteractUI = GetComponentInChildren<InteractHUD>();
        hitMarker = GetComponentInChildren<HitMarker>();

        InteractUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        OnEnemyHit += HandleHitMarker;
        OnEnemyKill += HandleKillFeed;
    }

    private void OnDisable()
    {
        OnEnemyHit -= HandleHitMarker;
        OnEnemyKill -= HandleKillFeed;
    }

    private void Update()
    {
        crosshair.gameObject.SetActive(Crosshair);
        hitMarker.enableKillMarker = KillMarker;
    }

    private void HandleHitMarker(bool isKill,bool showMarker)
    {
        hitMarker.Show(HitMarker && showMarker, HitMarkerSFX, isKill);
    }

    private void HandleKillFeed()
    {
        
    }
}
