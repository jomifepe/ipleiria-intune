using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Paused_Menu : MonoBehaviour
{
	[SerializeField] private GameObject HUD;
    [SerializeField] private Text levelNumberText;

    private void Awake()
    {
        levelNumberText.text = GameManager.Instance.GetCurrentLevel();
    }

    public void Resume()
    {
        //UIManager.Instance.ShowPausePanel(false);
		//HUD.SetActive(true);
        //Time.timeScale = 1f;
        Debug.Log("Resumed");
		GameManager.Instance.PauseGame(false);
    }

    public void Restart()
    {
        Debug.Log("Restart..");
        int level = GameManager.Instance.GetLevel();
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(--level));
        HUD.SetActive(true);
		GameManager.Instance.PauseGame(false);
    }

    public void QuitLevel()
    {
        UIManager.Instance.NotEndGame();
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(0));
        //Time.timeScale = 1f;
		GameManager.Instance.PauseGame(false);
    }
    
    public void Option()
    {
        Time.timeScale = 0f;
        Debug.Log("Option");
    }

    private IEnumerator Exit()
    {
        yield return null;
    }
}
