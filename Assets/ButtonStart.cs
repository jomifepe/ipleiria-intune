using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    public void LoadLevel()
    {
        GameManager.Instance.LoadNextLevel();
    }
}
