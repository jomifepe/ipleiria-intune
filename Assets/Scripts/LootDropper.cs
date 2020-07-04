using System;
using UnityEngine;
using Random = System.Random;

public class LootDropper : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject lifePrefab;
    [SerializeField] private GameObject throwChargePrefab;
    [SerializeField] private int valueMultiplier = 1;
    [Range(0,1)] [SerializeField] private float lifeDropChance = 0.5f;
    [Range(0,1)] [SerializeField] private float throwChargeDropChance = 0.3f;
    [SerializeField] private int minCoinValue = 2;
    [SerializeField] private int maxCoinValue = 8;
    [SerializeField] private int minCoinCount = 1;
    [SerializeField] private int maxCoinCount = 1;
    
    private Random rng;

    private void Awake()
    {
        rng = new Random();
    }

    /* function called on death animation event */
    public void DropNormalLoot()
    {
        DropCoins();
        DropLife();
        DropThrowCharge();
    }

    private void DropCoins()
    {
        var tr = transform;
        var pos = tr.position;
        
        for (int i = 0; i < maxCoinCount; i++)
        {
            float posModifier = (float) (rng.NextDouble() * 1f);
            if (rng.Next(0, 2) == 1) posModifier *= -1f;
            float posX = i == 0 ? tr.position.x : pos.x - posModifier;
            Vector3 position = new Vector3(posX, pos.y);
            GameObject spawnedObject = Instantiate(coinPrefab, position, tr.rotation);
            Collectible collectible = spawnedObject.GetComponent<Collectible>();
            collectible.SetValue(rng.Next(minCoinValue, maxCoinValue + 1) * valueMultiplier);
        }
    }

    /* chance of dropping a life if the player isn't full health */
    private void DropLife()
    {
        var gm = GameManager.Instance;
        if (gm.CurrentPlayerHealth >= gm.MaxPlayerHealth) return;
        if (rng.NextDouble() > lifeDropChance) return;
        var tr = transform;
        
        Instantiate(lifePrefab, tr.position, tr.rotation);
    }

    /* chance of dropping a throw charge */
    private void DropThrowCharge()
    {
        if (rng.NextDouble() > throwChargeDropChance) return;
        var tr = transform;
        
        Instantiate(throwChargePrefab, tr.position, tr.rotation);
    }
}
