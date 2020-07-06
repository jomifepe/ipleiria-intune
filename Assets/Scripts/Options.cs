using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
	[SerializeField] private Slider slide;
	private float soundVolume;
	
	void Start() {
		//Time.timeScale = 1;
		soundVolume = AudioManager.Instance.GetVolume();
		//slide = GetComponent<Slider>();
		Debug.Log(soundVolume);
		slide.value = normalize(soundVolume);
	}

	private void Update()
	{
		//Debug.Log("Sound Volume: " + soundVolume);
		//slide.value = soundVolume;
	}
	
	public void SliderChange(float value)
	{
		//Debug.Log(value);
		AudioManager.Instance.ChangeMasterVolume(denormalize(value));
	}

	public void GoBack(){
		Debug.Log("Back");
		SceneManager.LoadScene("Menu");
	}

	public void OpenCredits()
	{
		StartCoroutine(GameManager.Instance.LoadNextLevelAsync(-3));
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
