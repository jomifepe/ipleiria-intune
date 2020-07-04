using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } = null;

    [SerializeField] private Text coinText;
    [SerializeField] private Image lifeBar;
    [SerializeField] private Image throwBar;
    [SerializeField] private GameObject pausePanel;
    private float playerMaxHealth = 3f;
    private float playerMaxThrows = 3f;
    
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

    public void UpdateCoins(int score)
    {
        coinText.text = score.ToString();
    }

    public void UpdatePlayerLife(float life)
    {
        lifeBar.fillAmount = life / playerMaxHealth;
    }
    
    public void UpdatePlayerThrows(float amount)
    {
        throwBar.fillAmount = amount / playerMaxThrows;
    }

    public void ShowPausePanel(bool value)
    {
        pausePanel.SetActive(value);
    }

    public void SetPlayerMaxHealth(float value)
    {
        if (value < 0) return;
        playerMaxHealth = value;
    }
    public void SetPlayerMaxThrows(float value)
    {
        if (value < 0) return;
        playerMaxThrows = value;
    }
}




