using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    private int score = 0;

    private int level = 1;

    public bool IsPaused { get; private set; } = false;

    private float oldTimeScale;

    [SerializeField]
    private GameObject HUD;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateGameScoreText();
    }

    private void UpdateGameScoreText()
    {
        UIManager.Instance.UpdateScore(score);
    }

    public void AddScore(int scoreToAdd)
    {
        if (scoreToAdd > 0)
        {
            score += scoreToAdd;
            UpdateGameScoreText();
        }
    }

    public void LoadNextLevel()
    {
        //SceneManager.LoadScene(1);
        StartCoroutine(LoadNextLevelAsync(level));
        level++;
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadNextLevelAsync(0));
        level = 1;
    }

    private void ResetScore()
    {
        score = 0;
        UpdateGameScoreText();
    }

    private IEnumerator LoadNextLevelAsync(int levelToLoad)
    {      
        HUD.SetActive(false);

        EnemyManager.Instance.ResetEnemyCounter();
        AsyncOperation asyncLoad;
        if (levelToLoad == 0)
        {
            ResetScore();
            asyncLoad = SceneManager.LoadSceneAsync("Menu");
        }
        else
        {
            if (Application.CanStreamedLevelBeLoaded("Level" + levelToLoad))
            {
                HUD.SetActive(true);
                asyncLoad = SceneManager.LoadSceneAsync("Level" + levelToLoad);                
            }
            else
            {               
                asyncLoad = SceneManager.LoadSceneAsync("TheEnd");
            }
        }

        while (!asyncLoad.isDone)
        {
            //print(asyncLoad.progress);
            yield return null;
        }
        yield return null;
    }

    public void PauseGame(bool pause)
    {
        IsPaused = pause;
        if (pause)
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            UIManager.Instance.ShowPausePanel(true);
            
        }
        else
        {
            Time.timeScale = oldTimeScale;
            UIManager.Instance.ShowPausePanel(false);
        }
    }
}
