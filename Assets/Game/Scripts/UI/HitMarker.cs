using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    public AudioClip hitSoundEffect;
    public float despawnTime;

    private void Awake()
    {
        SoundManager.instance.PlaySound(hitSoundEffect, .08f, .15f, true, 0);

        Destroy(gameObject, despawnTime);
    }
}
