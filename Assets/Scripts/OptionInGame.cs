using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionInGame : MonoBehaviour
{
    private float soundVolume;
<<<<<<< HEAD:Assets/OptionInGame.cs
    private GameObject volumeSliderObject;
=======
    [SerializeField] private Slider slide;
>>>>>>> master:Assets/Scripts/OptionInGame.cs

    private void Awake()
    {
        soundVolume = AudioManager.Instance.GetVolume();
<<<<<<< HEAD:Assets/OptionInGame.cs
        volumeSliderObject = GameObject.Find("Slider");
        //Slider volumeSlider = volumeSliderObject.GetComponent<Slider>();
=======
        Debug.Log("sound" + soundVolume);
        slide.value = normalize(soundVolume);
    }
    
    /*void Start()
    {
        slide = GetComponent<Slider>();
>>>>>>> master:Assets/Scripts/OptionInGame.cs
    }

    private void Update()
    {
        slide.value = soundVolume;
    }*/

    public void SliderChange(float value)
    {
         AudioManager.Instance.ChangeMasterVolume(denormalize(value));
    }
    
    private float normalize(float value)
    {
        float min = -80;
        float max = 0;

        float normalized = (value - min) / (max - min);
        Debug.Log("Normalize value: " + normalized);
        return normalized;
    }

    private float denormalize(float value)
    {
        float min = -80;
        float max = 0;

        float denormalize = (value * (max - min) + min);
        Debug.Log("Denormalize value: " + denormalize);
        return denormalize;
    }
    
}
