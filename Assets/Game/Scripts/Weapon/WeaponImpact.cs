using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponImpact : MonoBehaviour,IPoolingObject
{
    [SerializeField] PoolTag poolTag;
    public PoolTag Tag { get => poolTag; }
    public float destroyTime;

    public ParticleSystem impactParticle;

    public AudioClip[] impactSounds;
    public AudioSource audioSource;

    public void OnObjectSpawn()
    {
        PoolManager.instance.DespawnGameObject(gameObject, destroyTime);

        impactParticle.Play();

        audioSource.clip = impactSounds
            [Random.Range(0, impactSounds.Length)];
        audioSource.Play();
    }
}
