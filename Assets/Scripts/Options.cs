using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{
	void Start() {
		Time.timeScale = 1;
	}

	public void GoBack(){
		Debug.Log("Back");
		SceneManager.LoadScene("Menu");
	}

	public void OpenCredits()
	{
		StartCoroutine(GameManager.Instance.LoadNextLevelAsync(-3));
		
	}
}
