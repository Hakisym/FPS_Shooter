using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponImpact : MonoBehaviour
{
    public float destroyTime;

    public AudioClip[] impactSounds;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);

        audioSource.clip = impactSounds
            [Random.Range(0, impactSounds.Length)];
        audioSource.Play();
    }
}
