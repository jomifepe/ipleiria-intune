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

    protected override void Init()
    {
        life = maxHealth = 2f;
        minCoinDrop = 5;
        maxCoinDrop = 10;
        minCoinCount = 1;
        maxCoinCount = 2;
        sensingRange = 10f;
    }

    /*protected override void EnemyMove()
    {
        Debug.Log("Skeleton enemy move");
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
        if (!PlayerOnSensingRange(direction.x))
        {
            // Debug.Log("Player isn't on sensing range");
            inRange = false;
            return;
        }

        if (diffPlatforms) return;

        //Debug.Log(transform.localEulerAngles.y);
        //Debug.Log(direction.x);

        inRange = true;
        UpdateMovement(direction);
    }

    protected override void EnemyFixedUpdate()
	{
        Debug.Log("Skeleton fixed update");
        if (attackMode) return;
        if (diffPlatforms || !inRange)
        {
            MoveNormally();
            return;
        }

        UpdateReachedBorder();
        //following the player
        if (!reachedBorder) FollowPlayer();
    }*/

    protected override void Attack()
	{
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(meleeAttackPoint.position, attackRange, playerLayerMask);
        
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }
}
