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

    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject controls;

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
    
     public void LoadLevel(int level)
    {
        StartCoroutine(LoadNextLevelAsync(level));
        this.level = ++level;
    }

    public void LoadMenuLevels()
    {
        StartCoroutine(LoadNextLevelAsync(-1));
        level++;
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadNextLevelAsync(0));
        level = 1;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateGameScoreText();
    }

    public IEnumerator LoadNextLevelAsync(int levelToLoad)
    {      
        HUD.SetActive(false);

        EnemyManager.Instance.ResetEnemyCounter();
        AsyncOperation asyncLoad;
        if (levelToLoad == 0)
        {
            ResetScore();
            asyncLoad = SceneManager.LoadSceneAsync("Menu");
        }
        else if (levelToLoad == -1)
        {
            ResetScore();
            asyncLoad = SceneManager.LoadSceneAsync("Menu_Levels");
        }
        else if (levelToLoad == -2)
        {
            ResetScore();
            asyncLoad = SceneManager.LoadSceneAsync("Option");
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

    public int GetLevel()
    {
        return level;
    }

    public void PauseGame(bool pause)
    {
        Debug.Log("Press paused");
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
