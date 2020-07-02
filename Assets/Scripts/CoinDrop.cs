using System;
using UnityEngine;
using Random = System.Random;

public class CoinDrop : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int valueMultiplier = 1;
    private Random rng;

    private void Awake()
    {
        rng = new Random();
    }

    public void DropCoins(int minValue, int maxValue, int minCount, int maxCount)
    {
        var tr = transform;
        for (int i = 0; i < maxCount; i++)
        {
            float posModifier = (float) (rng.NextDouble() * 1f);
            if (rng.Next(0, 2) == 1) posModifier *= -1f;
            float posX = i == 0 ? tr.position.x : tr.position.x - posModifier;
            Vector3 position = new Vector3(posX, tr.position.y);
            GameObject spawnedObject = Instantiate(coinPrefab, position, tr.rotation);
            Coin coin = spawnedObject.GetComponent<Coin>();
            coin.setCoinValue(rng.Next(minValue, maxValue + 1) * valueMultiplier);
        }
    }
}
