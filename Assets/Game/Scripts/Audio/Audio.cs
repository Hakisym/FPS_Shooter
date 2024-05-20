using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Audio
{
    public AudioProfile audioProfile;
    public AudioSource audioSource;
    public GameObject target;

    public void Equip(GameObject target, AudioProfile profile)
    {
        if (audioSource) return;
        this.target = target;
        audioSource = target.AddComponent<AudioSource>();
        audioProfile = profile;
        Update(profile);
    }

    public void Update(AudioProfile audioProfile)
    {
        this.audioProfile = audioProfile;
        if (!audioSource || audioSource && !audioProfile) return;
        audioSource.clip = audioProfile.clip;
        audioSource.outputAudioMixerGroup = audioProfile.output;
        audioSource.mute = audioProfile.mute;
        audioSource.bypassEffects = audioProfile.bypassEffects;
        audioSource.bypassListenerEffects = audioProfile.bypassListenerEffects;
        audioSource.bypassReverbZones = audioProfile.bypassReverbZones;
        audioSource.playOnAwake = audioProfile.playOnAwake;
        audioSource.loop = audioProfile.loop;

        audioSource.priority = audioProfile.priority;
        audioSource.volume = audioProfile.volume;
        audioSource.pitch = audioProfile.pitch;
        audioSource.panStereo = audioProfile.stereoPan;
        audioSource.spatialBlend = audioProfile.spatialBlend;
        audioSource.reverbZoneMix = audioProfile.reverbZoneMix;

        audioSource.dopplerLevel = audioProfile.dopplerLevel;
        audioSource.spread = audioProfile.spread;
        audioSource.rolloffMode = audioProfile.volumeRolloff;
        audioSource.minDistance = audioProfile.minDistance;
        audioSource.maxDistance = audioProfile.maxDistance;
    }

    public void PlayOneShot(AudioProfile profile)
    {
        Equip(target, profile);
        Update(profile);
        RandomizePitch();

        if (!profile.clip)
        {
            return;
        }

        audioSource?.PlayOneShot(profile.clip);
    }

    private void RandomizePitch()
    {
        audioSource.pitch = RandomizedPitch();
    }

    private float RandomizedPitch()
    {
        if (audioProfile.dynamicPitch)
            return Random.Range((Time.timeScale * audioProfile.pitch), (Time.timeScale + audioProfile.pitchFactor) * audioProfile.pitch);

        return audioProfile.pitch;
    }
}
