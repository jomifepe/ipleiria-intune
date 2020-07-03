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
        Life = 2f;
        maxLife = Life;
        sensingRange = 10f;
    }

    protected override void EnemyMove()
    {
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
        if (!PlayerOnSensingRange(direction.x))
        {
            Debug.Log("Player isnt on sensing range");
            InRange = false;
            return;
        }

        if (diffPlatforms) return;

        //Debug.Log(transform.localEulerAngles.y);
        //Debug.Log(direction.x);

        InRange = true;
        UpdateMovement(direction);
    }

    protected override void EnemyFixedUpdate()
	{
        if (attackMode) return;
        if (diffPlatforms || !InRange)
        {
            MoveNormally();
            return;
        }

        UpdateReachedBorder();
        //following the player
        if (!ReachedBorder) FollowPlayer();
    }

    protected override void Attack()
	{
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(meleeAttackPoint.position, attackRange, playerLayerMask);
        
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerController>().TakeDamage(attackDamage);
        }
    }
}
