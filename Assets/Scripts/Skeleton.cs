using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Skeleton: Enemy
{
    [SerializeField] private Transform meleeAttackPoint;
    [SerializeField] private LayerMask playerLayerMask;
    
    private readonly Collider2D[] results = new Collider2D[1];
    
    protected override void Init()
    {
        life = maxHealth = 2f;
        sensingRange = 10f;
    }
    
    protected override void Attack()
    {
        if (Physics2D.OverlapCircleNonAlloc(meleeAttackPoint.position, attackRange, results, playerLayerMask) == 1)
        {
            results[0].GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }
}
