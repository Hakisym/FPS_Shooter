using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Interact")] 
    public Transform InteractUI;
    public TextMeshProUGUI interactionName;

    public static UIManager instance;

    private void Start()
    {
        instance = this;
    }
}
