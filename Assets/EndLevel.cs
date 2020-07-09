using UnityEngine;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("End level");
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.LevelCompleted();
        }
    }
}
