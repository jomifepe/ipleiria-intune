using UnityEngine;

public class ButtonStart : MonoBehaviour
{
    public void LoadLevel()
    {
        GameManager.Instance.LoadMenuLevels();
        Debug.Log("Press");
    }
}
