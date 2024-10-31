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

    public InteractHUD InteractUI;
    public PauseMenu pauseMenu;
    public HitMarker hitMarker;
    public KillFeed killFeed;
    public DamageIndicator damageIndicator;
    public RespawnPanel respawnPanel;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;

        InteractUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            crosshair.gameObject.SetActive(!crosshair.gameObject.activeSelf);
        }
    }
}
