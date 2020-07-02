using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;

	void Start() {
		Time.timeScale = 1;
	}

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Image lifeBar;

    [SerializeField]
    private Text enemyCounterText;

    [SerializeField]
    private GameObject pausePanel;

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

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void UpdatePlayerLife(float life)
    {
        lifeBar.fillAmount = life / 3f;
    }

    public void UpdateEnemyCounter(int enemyCounter)
    {
        if (enemyCounterText != null)
        {
            enemyCounterText.text = enemyCounter.ToString();
        }
    }

    public void ShowPausePanel(bool value)
    {
        pausePanel.SetActive(value);
    }
	
	public void OpenOptionMenu()
	{
		Debug.Log("[UIManager] Click Open options");
        SceneManager.LoadScene("Option_inMenu");
	}
}




