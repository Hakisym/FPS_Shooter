using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text healthText;

    public void SetHealthValue(float value)
    {
        healthText.text = value.ToString();
    }
}
