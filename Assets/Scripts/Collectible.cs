using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private enum Type
    {
        Coin, Heart, Throw
    }
    
    [SerializeField] private Type type;
    [SerializeField] private int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool shouldDismiss = true;
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            switch (type)
            {
                case Type.Coin: 
                    GameManager.Instance.IncrementCoins(value);
                    break;
                case Type.Heart:
                    if (!player.IsFullHealth()) player.GiveHealth(value);
                    else shouldDismiss = false;
                    break;
                case Type.Throw:
                    if (!player.HasAllThrows()) player.GiveThrow(value);
                    else shouldDismiss = false;
                    break;
            }

            if (shouldDismiss) Dismiss();
        }
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    private void Dismiss()
    {
        Destroy(gameObject);
    }
}
