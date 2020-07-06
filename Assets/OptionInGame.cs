using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionInGame : MonoBehaviour
{
    private float soundVolume;
    private GameObject volumeSliderObject = GameObject.Find("Slider");

    private void Awake()
    {
        soundVolume = AudioManager.Instance.GetVolume();
        //Slider volumeSlider = volumeSliderObject.GetComponent<Slider>();
    }

    public void SliderChange(float value)
    {
         Debug.Log(value);
         AudioManager.Instance.ChangeMasterVolume(value);
    }
    
}
