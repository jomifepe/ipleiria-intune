using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Paused_Menu : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.PauseGame(false);
    }

    public void Restart()
    {
        Debug.Log("Restart..");
        int level = GameManager.Instance.GetLevel();
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(--level));
        Time.timeScale = 1f;
    }

    public void QuitLevel()
    {
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(0));
        Time.timeScale = 1f;
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
