using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    public AudioClip hitSoundEffect;
    public Image hitMark;
    private Animator animator;
    public float despawnTime;

    private void Awake()
    {
        animator = hitMark.GetComponent<Animator>();
    }

    public void Show(bool enableHUD, bool enableSFX, bool killed)
    {
        if (enableHUD)
        {
            hitMark.color = killed ? Color.red : Color.white;

            animator.Play("Hit", 0, 0f);
        }
        
        if(enableSFX) SoundManager.instance.PlaySound(hitSoundEffect, .08f, .15f, true, 0);

    }
}
