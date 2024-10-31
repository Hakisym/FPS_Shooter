using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RespawnPanel : MonoBehaviour
{
    public Image respawnBackground;
    [SerializeField] private TMP_Text respawnText;
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetRespawnTimeText(float value)
    {
        if (value <= 0)
        {
            respawnText.text = "Respawn 0.00";
        }
        else
        {
            respawnText.text = "Respawn " + value.ToString("F2");
        }
    }

    public void StartRespawnTime(float value)
    {
        StartCoroutine(RespawnCoroutine(value));
    }

    IEnumerator RespawnCoroutine(float value)
    {
        canvasGroup.alpha = 1;
        while (value > 0)
        {
            value -= Time.deltaTime;
            SetRespawnTimeText(value);
            yield return null;
        }
        SetRespawnTimeText(value);
        canvasGroup.alpha = 0;
    }
}
