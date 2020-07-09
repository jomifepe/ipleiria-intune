using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Model;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } = null;
    private const string StateKeyCoins = "coins";
    
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject controls;
    [SerializeField] private List<Song> songList;
    private String currentLevel;
    
    public float CurrentPlayerHealth { get; private set; }
    public float CurrentPlayerThrows { get; private set; }
    public float MaxPlayerHealth { get; private set; }
    public float MaxPlayerThrows { get; private set; }

    public (float, float) platformBounds;

    private int levelCoins, originalCoins;

    private int TotalCoins => originalCoins + levelCoins;

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(StateKeyCoins, TotalCoins);
        PlayerPrefs.Save();            
    }

    private int level = 1;

    public bool IsPaused { get; set; }

    private float oldTimeScale;
    public Song CurrentSong { get; private set; }
    private int currentSongIndex = -1;
    private Dictionary<Buff, (Sprite throwBar, Sprite throwButton)> buffResources;

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
        StartDefaultSong();
        if (PlayerPrefs.HasKey(StateKeyCoins))
        {
            originalCoins = PlayerPrefs.GetInt(StateKeyCoins);
        }
        UpdateCoinsText();
        buffResources = new Dictionary<Buff, (Sprite throwBar, Sprite throwButton)>
        {
            {Buff.None, (
                throwBar: Resources.Load<Sprite>("UI/ThrowBarFill"),
                throwButton: Resources.Load<Sprite>("UI/ranged_attack")
            )},
            {Buff.Physical, (
                throwBar: Resources.Load<Sprite>("UI/ThrowBarFillBloody"),
                throwButton: Resources.Load<Sprite>("UI/ranged_attack_bloody")
            )},
            {Buff.Slow, (
                throwBar: Resources.Load<Sprite>("UI/ThrowBarFillIcy"),
                throwButton: Resources.Load<Sprite>("UI/ranged_attack_icy")
            )}
        };
    }

    private void StartDefaultSong()
    {
        if (songList.Count == 0) return;
        AudioManager.Instance.ChangeSong(songList[0].clip, false);
        currentSongIndex = 0;
    }
    
    private void UpdateCoinsText()
    {
        UIManager.Instance.UpdateCoins(TotalCoins);
    }

    public void IncrementCoins(int amount)
    {
        if (amount <= 0) return;
        levelCoins += amount;
        UpdateCoinsText();
    }


    public void LoadNextLevel()
    {
        //SceneManager.LoadScene(1);
        PauseGame(false, false);
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

    public IEnumerator LoadNextLevelAsync(int levelToLoad)
    {      
        HUD.SetActive(false);

        AsyncOperation asyncLoad;
        if (levelToLoad == 0)
        {
            asyncLoad = SceneManager.LoadSceneAsync("Menu");
        }
        else if (levelToLoad == -1)
        {
            asyncLoad = SceneManager.LoadSceneAsync("Menu_Levels");
        }
        else if (levelToLoad == -2)
        {
            asyncLoad = SceneManager.LoadSceneAsync("Option");
        }
        else if (levelToLoad == -3)
        {
            //ResetScore();
            asyncLoad = SceneManager.LoadSceneAsync("Credits");
        }
        else
        {
            if (Application.CanStreamedLevelBeLoaded("Level" + levelToLoad))
            {
                HUD.SetActive(true);
                asyncLoad = SceneManager.LoadSceneAsync("Level" + levelToLoad);
                currentLevel = "Level" + levelToLoad;
            }
            else
            {               
                asyncLoad = SceneManager.LoadSceneAsync("TheEnd"); 
                // possivelmente não será assim.
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

    public void PauseGame(bool pause, bool showPanel = true)
    {
        Debug.Log("Press paused");
        IsPaused = pause;
        if (pause)
        {
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            if(showPanel) UIManager.Instance.ShowPausePanel(true);
        }
        else
        {
            Time.timeScale = oldTimeScale;
            if(showPanel) UIManager.Instance.ShowPausePanel(false);
        }
    }

    public void PauseGameButton(bool pause)
    {
        PauseGame(pause);
    }

    public void SetPlayerMaxHealth(float value)
    {
        MaxPlayerHealth = value;
        UIManager.Instance.SetPlayerMaxHealth(value);
    }

    public void SetPlayerMaxThrows(float value)
    {
        MaxPlayerThrows = value;
        UIManager.Instance.SetPlayerMaxThrows(value);
    }

    public void UpdatePlayerLife(float value)
    {
        CurrentPlayerHealth = value;
        UIManager.Instance.UpdatePlayerLife(value);
    }

    public void UpdatePlayerThrows(float value)
    {
        CurrentPlayerThrows = value;
        UIManager.Instance.UpdatePlayerThrows(value);
    }
    
    public void AddSong(Song song)
    {
        /* make the button visible if it's the first song that's been collected */
        if (songList.Count == 1) UIManager.Instance.ChangeSongButtonVisibility(true);
        songList.Add(song);
    }
    
    public void ChangeSong()
    {
        currentSongIndex++;
        if (currentSongIndex == songList.Count) currentSongIndex = 0; // repeat the song list
        CurrentSong = songList[currentSongIndex];
        AudioManager.Instance.ChangeSong(CurrentSong.clip);
        UIManager.Instance.SetThrowBarImage(buffResources[CurrentSong.buff].throwBar);
        UIManager.Instance.SetThrowButtonImage(buffResources[CurrentSong.buff].throwButton);
    }
    
    /*public void OpenOptionsOnMenu()
    {
        Debug.Log("[GAMEMANAGER] Click Open options");
        UIManager.Instance.OpenOptionMenu();
    }*/
    
    public String GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public int GetCoins()
    {
        return TotalCoins;
    }
    
    public int GetLevelCoins()
    {
        return levelCoins;
    }
    
    // Para remover
    /*public void ResetGame()
    {
        Debug.Log("[GameManager] Restart..");
        StartCoroutine(GameManager.Instance.LoadNextLevelAsync(--level));
        PauseGame(false);
        
        // Vida -- Auto, não precisa
        // machados
        
        // coins
        
        // discos
        
        
    }*/

    public void LevelCompleted()
    {
        PauseGame(true, false);
        SaveCoins();
        UIManager.Instance.LevelCompleted();
    }
    
    public void EndGame(bool winner)
    {
        PauseGame(true, false);
        UIManager.Instance.EndGame(winner);
    }

    public void RestartSameLevel()
    {
        LoadNextLevelAsync(level);
    }
}
