﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused_Menu : MonoBehaviour
{
	[SerializeField] private GameObject HUD;

    public void Resume()
    {
        //UIManager.Instance.ShowPausePanel(false);
		//HUD.SetActive(true);
        //Time.timeScale = 1f;
        Debug.Log("Reseumed");
		GameManager.Instance.PauseGame(false);
    }

    public void Restart()
    {
        Debug.Log("Restart..");
        int level = GameManager.Instance.GetLevel();
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(--level));
		GameManager.Instance.PauseGame(false);
        //Time.timeScale = 1f;
    }

    public void QuitLevel()
    {
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(0));
        //Time.timeScale = 1f;
		GameManager.Instance.PauseGame(false);
    }
    
    public void Option()
    {
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(-2)); // Option
        Time.timeScale = 1f;
    }

    private IEnumerator Exit()
    {
        yield return null;
    }
}
