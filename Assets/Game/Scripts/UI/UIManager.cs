using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Interact")] 
    public Transform InteractUI;
    public TextMeshProUGUI interactionName;

    [Header("AimDot")]
    public Transform aimDot;

    public static UIManager instance;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse4))
        {
            aimDot.gameObject.SetActive(!aimDot.gameObject.activeSelf);
        }
    }
}
