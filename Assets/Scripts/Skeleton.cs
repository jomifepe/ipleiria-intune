using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Skeleton: Enemy
{
    private float maxHealth = 2f;
    private float sensingRange = 10f;

    private bool diffPlatforms = false;
    private float triggerPosition = -1f;
    protected bool right = true;

    protected override float getMaxHealth()
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
        direction = player.position - transform.position;
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

    /*protected override void enemyUpdate()
	{
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
        direction = player.position - transform.position;
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
    }*/

    private bool isOnAnotherPlatform(float playerPosition)
	{
        return (player.position.x < triggerPosition && right) ||
        (player.position.x > triggerPosition && !right);
    }

    protected override void enemyFixedUpdate()
	{
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
        //Debug.Log("Follow player normaly");
        if (CanFlip) Flip();
        moveCharacter(movement);
        return;
    }
}
