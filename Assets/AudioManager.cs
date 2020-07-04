using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } = null;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMasterVolume(float volumeValue)
    {
        audioMixer.SetFloat("MasterVolume", volumeValue);
    }

    public void ChangeCurrentSong(AudioClip song)
    {
        // TODO
    }

    public void PreviewSong(AudioClip song)
    {
        // TODO
    }
}
