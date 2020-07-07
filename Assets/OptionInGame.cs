using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionInGame : MonoBehaviour
{
    private float soundVolume;
    private GameObject volumeSliderObject;

    private void Awake()
    {
        soundVolume = AudioManager.Instance.GetVolume();
        volumeSliderObject = GameObject.Find("Slider");
        //Slider volumeSlider = volumeSliderObject.GetComponent<Slider>();
    }

    public void SliderChange(float value)
    {
         Debug.Log(value);
         AudioManager.Instance.ChangeMasterVolume(value);
    }
    
}
