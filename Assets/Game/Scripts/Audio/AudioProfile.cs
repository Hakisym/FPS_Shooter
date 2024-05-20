using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName ="New Audio Profile",menuName ="Audio/Audio Profile")]
public class AudioProfile : ScriptableObject
{
    public AudioClip clip;
    public AudioMixerGroup output;
    public bool mute;
    public bool bypassEffects;
    public bool bypassListenerEffects;
    public bool bypassReverbZones;
    public bool playOnAwake = false;
    public bool loop;

    [Space]
    [Range(0, 256)] public int priority = 128;
    [Range(0, 1)] public float volume = 1;
    [Range(-3, 3)] public float pitch = 1;
    [Range(-1, 1)] public float stereoPan = 0;
    [Range(0, 1)] public float spatialBlend = 0;
    [Range(0, 1.1f)] public float reverbZoneMix = 1;

    [Header("3D Sound Settings")]
    [Range(0, 5)] public float dopplerLevel = 1;
    [Range(0, 360)] public float spread = 0;
    public AudioRolloffMode volumeRolloff = AudioRolloffMode.Custom;
    public float minDistance = 1;
    public float maxDistance = 500;

    [Space]
    public bool dynamicPitch = true;
    public float pitchFactor = 0.2f;

}
