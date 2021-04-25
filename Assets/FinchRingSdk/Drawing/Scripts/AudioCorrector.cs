using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioCorrector : MonoBehaviour
{
    public static int AudioPower { get { return power; } set { power = Mathf.Clamp(value, 0, 10); } }
    public static float AudioPower01 { get { return power / 10.0f; } }
    public static bool Mute;

    private static int power = 5;

    [Range(0, 1)]
    public float BasePower = 1;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        audioSource.Play();
    }


  
    private void Update()
    {
        // Redudant Audio system - look at ULauncherAudio
         audioSource.volume = Mute ? 0 : (BasePower * AudioPower / 10);
    }
}
