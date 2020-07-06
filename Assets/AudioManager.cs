using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } = null;

    [SerializeField] private AudioMixer audioMixer;
    private float volumeSong = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (audioMixer != null)
            {
                audioMixer.GetFloat("MasterVolume", out volumeSong);
            }
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

    public float GetVolume()
    {
        return volumeSong;
    }
}
