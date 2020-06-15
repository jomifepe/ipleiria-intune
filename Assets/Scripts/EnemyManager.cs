using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; } = null;
    private int enemiesCounter = 0;

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

    public void IncreaseEnemiesCounter()
    {
        enemiesCounter++;
        UpdateEnemiesCounterText();
    }

    public void DecreaseEnemiesCounter()
    {
        enemiesCounter--;
        UpdateEnemiesCounterText();
    }

    private void UpdateEnemiesCounterText()
    {
        UIManager.Instance.UpdateEnemyCounter(enemiesCounter);
    }

    public void ResetEnemyCounter()
    {
        enemiesCounter = 0;
        UpdateEnemiesCounterText();
    }
}
