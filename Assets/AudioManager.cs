﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } = null;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] public AudioSource audioSource;
    
    private float volumeSong = 1f;
    private Coroutine currentCoroutine;
    private Boolean isSFXEnable = true;

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
        this.volumeSong = volumeValue;
    }

    public void PreviewSong(AudioClip song)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(PreviewSongAsync(song));
    }
    
    public void ChangeSong(AudioClip song, bool fade = true)
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ChangeCurrentSongAsync(song, fade));
    }
    
    private IEnumerator ChangeCurrentSongAsync(AudioClip song, bool fade)
    {
        var currentVolume = audioSource.volume;
        if (fade) yield return FadeOut(.02f);
        audioSource.clip = song;
        audioSource.time = 0;
        audioSource.Play(); 
        if (fade) yield return FadeIn(.02f);
    }

    private IEnumerator PreviewSongAsync(AudioClip song)
    {
        var previousSong = audioSource.clip;
        var previousTime = audioSource.time;

        yield return FadeOut(.03f);
        audioSource.clip = song;
        audioSource.time = song.length / 2f;
        audioSource.Play(); 
        yield return FadeIn();
        
        yield return new WaitForSeconds(song.length < 2f ? song.length : 2f);

        yield return FadeOut(.03f);
        audioSource.clip = previousSong;
        audioSource.time = previousTime;
        audioSource.Play(); 
        yield return FadeIn();
    }
    
    private IEnumerator FadeOut(float speed = .05f)
    {
        while (audioSource.volume > .1f)
        {
            audioSource.volume -= .1f;
            yield return new WaitForSeconds(speed);
        }
    }
    
    private IEnumerator FadeIn(float speed = .05f)
    {
        while (audioSource.volume < 1f)
        {
            audioSource.volume += .1f;
            yield return new WaitForSeconds(speed);
        }

        if (audioSource.volume > 1f) audioSource.volume = 1f;
    }

    public float GetVolume()
    {
        return volumeSong;
    }
    
    /*public void SetVolume(float value)
    {
        volumeSong = value;
    }*/
    public void SFX(bool toggleButton)
    {
        if (toggleButton)
        {
            audioMixer.SetFloat("SFX", 0f);
            isSFXEnable = true;
        }
        else
        {
            audioMixer.SetFloat("SFX", -80f);
            isSFXEnable = false;
        }
    }

    public Boolean GetIsSFXEnable()
    {
        return isSFXEnable;
    }
}
