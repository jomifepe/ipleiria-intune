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
    
    private float volumeSong = 1f;
    private Coroutine currentCoroutine;

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
        if (fade) yield return FadeOut(0.02f);
        audioSource.clip = song;
        audioSource.time = 0;
        audioSource.Play(); 
        if (fade) yield return FadeIn(currentVolume, 0.02f);
    }

    private IEnumerator PreviewSongAsync(AudioClip song)
    {
        var currentVolume = audioSource.volume;
        var previousSong = audioSource.clip;
        var previousTime = audioSource.time;

        yield return FadeOut(0.03f);
        audioSource.clip = song;
        audioSource.time = song.length / 2f;
        audioSource.Play(); 
        yield return FadeIn(currentVolume);
        
        yield return new WaitForSeconds(song.length < 2f ? song.length : 2f);

        yield return FadeOut(0.03f);
        audioSource.clip = previousSong;
        audioSource.time = previousTime;
        audioSource.Play(); 
        yield return FadeIn(currentVolume);
    }
    
    private IEnumerator FadeOut(float speed = 0.05f)
    {
        while (audioSource.volume > 0.1)
        {
            audioSource.volume -= 0.1f;
            yield return new WaitForSeconds(speed);
        }
    }
    
    private IEnumerator FadeIn(float maxVolume, float speed = 0.05f)
    {
        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += 0.1f;
            yield return new WaitForSeconds(speed);
        }

        if (audioSource.volume > maxVolume) audioSource.volume = maxVolume;
    }

    public float GetVolume()
    {
        return volumeSong;
    }
}
