using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Random = UnityEngine.Random;


[System.Serializable]
public class AudioClipDescription
{
    public AudioClip Clip;
    public float Volume = 1;

    public float PitchMin = 1;
    public float PitchMax = 1;
}


public class AudioMaster : MonoBehaviour
{
    public static AudioMaster Instance;

    [Header("Sounds")]
    public AudioClipDescription reintegration;
    public AudioClipDescription bounce;

    public void Awake()
    {
        Instance = this;
    }
	
    public static void Play(AudioClipDescription audioClip)
    {
        GameObject goAudio = new GameObject("Audio ["+audioClip.Clip.name + "]");
        goAudio.transform.parent = Instance.transform;

        AudioSource source = goAudio.AddComponent<AudioSource>();
        source.spatialBlend = 1;
        source.pitch = Random.Range(audioClip.PitchMin, audioClip.PitchMax);
        source.clip = audioClip.Clip;
        source.volume = audioClip.Volume; //* AudioCorrector.AudioPower01;
        source.Play();

        Destroy(source, audioClip.Clip.length);
    }
}
