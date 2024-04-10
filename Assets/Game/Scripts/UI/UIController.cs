using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Interact")] 
    public Transform InteractUI;
    public TextMeshProUGUI interactionName;

    [Header("DebugDot")]
    public Transform debugDot;

    [Header("CrossHair")]
    public CrossHair crosshair;

    public static UIController instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            debugDot.gameObject.SetActive(!debugDot.gameObject.activeSelf);
        }
    }
}
