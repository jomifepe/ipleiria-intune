using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int coinScore = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.IncrementCoins(coinScore);
            Dismiss();
        }
    }

    public void setCoinValue(int value)
    {
        coinScore = value;
    }

    private void Dismiss()
    {
        Destroy(gameObject);
    }
}
