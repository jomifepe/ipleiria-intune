using UnityEngine;

public class Button_Level1 : MonoBehaviour
{
    public void LoadLevel()
    {
        GameManager.Instance.LoadLevel(1);
    }
}
