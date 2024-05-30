using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    public AudioClip hitSoundEffect;
    public RectTransform hitMarkContainer;

    public Vector3 hitMarkerSize;
    public Vector3 killMarkerSize;


    public void Show(bool enableHUD, bool enableSFX, bool critical,Vector3 position)
    {
        if (enableHUD)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(position);

            var container = Instantiate(hitMarkContainer, pos, Quaternion.identity, transform);
            Image hitMarker = container.GetComponentInChildren<Image>();

            container.localScale = critical ? killMarkerSize : hitMarkerSize;
            hitMarker.color = critical ? Color.red : Color.white;
            hitMarker.GetComponent<Animator>().Play("Hit", 0, 0f);
        }

        if (enableSFX) SoundManager.instance.PlaySound(hitSoundEffect, .08f, .15f, true, 0);

    }
}
