using UnityEngine;

public class SpriteRenderingOrderManager : MonoBehaviour
{
    public static SpriteRenderingOrderManager Instance { get; private set; } = null;

    private int enemyOrderInLayer = 0;

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

    public int GetEnemyOrderInLayer()
    {
        enemyOrderInLayer++;
        return enemyOrderInLayer;
    }
}
