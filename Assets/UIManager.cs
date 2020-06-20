using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;

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
}




