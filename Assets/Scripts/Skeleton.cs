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

    private float maxHealth = 2f;
    private float sensingRange = 10f;

    private bool diffPlatforms = false;
    private float triggerPosition = -1f;
    protected bool right = true;

    protected override float GetMaxHealth()
    {
        return maxHealth;
    }

    protected override float getSensingRange()
	{
        return sensingRange;
	}

    protected override void enemyMove()
    {
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
        updateCanFlip(direction);//change method name
        if (!isPlayerOnRange(direction.x))
        {       
            InRange = false;
            return;
        }

        if (diffPlatforms)
        {
            if (!isOnAnotherPlatform(player.position.x)) return;	
            //Debug.Log("Check trigger false");
            diffPlatforms = false;
        }

        //Debug.Log(transform.localEulerAngles.y);
        //Debug.Log(direction.x);

        InRange = true;
        updateMovement(direction);
    }

    private bool isOnAnotherPlatform(float playerPosition)
	{
        return (player.position.x < triggerPosition && right) ||
        (player.position.x > triggerPosition && !right);
    }

    protected override void enemyFixedUpdate()
	{
        if (attackMode) return;
        updateReachedBorder();
        if(!diffPlatforms && ReachedBorder && sameDirection(direction))
        {
            triggerPosition = transform.position.x;
            right = direction.x >= 0;
            diffPlatforms = true;

            Flip();
            return;
        }

        if (diffPlatforms || !InRange)
        {
            moveNormally();
            return;
        }

        //following the player
        if (!ReachedBorder) followPlayer();
    }

    private void followPlayer()
    {
        if (CanFlip) Flip();
        moveCharacter(movement);
        return;
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
