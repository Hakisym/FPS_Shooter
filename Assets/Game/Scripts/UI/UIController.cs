using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    [Header("Interact")] 
    public Transform InteractUI;
    public TextMeshProUGUI interactionName;

    [Header("DebugDot")]
    public Transform debugDot;

    [Header("CrossHair")]
    public CrossHair crosshair;

    [Header("HitMarker")]
    public GameObject hitMarker;
    public GameObject killMarker;

    public static UIController instance;
    public Action OnEnemyHit;
    public Action OnEnemyKill;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        OnEnemyHit += HitMarker;
        OnEnemyKill += KillMarker;
    }

    private void OnDisable()
    {
        OnEnemyHit -= HitMarker;
        OnEnemyKill -= KillMarker;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            debugDot.gameObject.SetActive(!debugDot.gameObject.activeSelf);
        }
    }

    private void HitMarker()
    {
        Instantiate(hitMarker, transform.position, Quaternion.identity, transform);
    }

    private void KillMarker()
    {
        Instantiate(killMarker, transform.position, Quaternion.identity, transform);
    }
}
