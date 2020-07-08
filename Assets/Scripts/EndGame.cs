using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Text coinText;
    [SerializeField] private Text discsText;
    private String currentLevel;
    
    private void Start()
    {
        currentLevel = GameManager.Instance.GetCurrentLevel();
        int coins = GameManager.Instance.GetCoins();
        coinText.text = coins.ToString();
        
        // FALTA os Discos
    }

    public void RestartGame()
    {
        UIManager.Instance.NotEndGame();
        SceneManager.LoadSceneAsync(currentLevel);
    }

    public void NextLevel()
    {
        UIManager.Instance.NotEndGame();
        GameManager.Instance.LoadNextLevel();
    }
}
