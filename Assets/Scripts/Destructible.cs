using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    enum Type
    {
        Crate, Barrel, Vase, Chest
    }
    
    private Animator animator;
    
    private bool isIntact = true;
    private static readonly int Destruct = Animator.StringToHash("Destruct");
    private CoinDrop coinDropper;
    [SerializeField] private Type type;
    private int minCoinValue;
    private int maxCoinValue;
    private int minCoinDrop;
    private int maxCoinDrop;


    private void Awake()
    {
        coinDropper = GetComponent<CoinDrop>();
        switch (type)
        {
            case Type.Crate:
                minCoinValue = 3;
                maxCoinValue = 5;
                minCoinDrop = 1;
                maxCoinDrop = 1;
                break;
            case Type.Barrel:
                minCoinValue = 2;
                maxCoinValue = 4;
                minCoinDrop = 1;
                maxCoinDrop = 2;
                break;
            case Type.Vase:
                minCoinValue = 3;
                maxCoinValue = 9;
                minCoinDrop = 1;
                maxCoinDrop = 1;
                break;
            case Type.Chest:
                minCoinValue = 3;
                maxCoinValue = 10;
                minCoinDrop = 2;
                maxCoinDrop = 4;
                break;
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Destroy()
    {
        if (isIntact)
        {
            isIntact = false;
            animator.SetTrigger(Destruct);
            coinDropper.DropCoins(minCoinValue, maxCoinValue, 
                minCoinDrop, maxCoinDrop);
            Invoke(nameof(DestroyObject), 3);
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
