using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Witch : Enemy
{
    protected override void Init()
    {
        life = maxHealth = 3f;
        minCoinDrop = 10;
        maxCoinDrop = 15;
        minCoinCount = 1;
        maxCoinCount = 2;
        sensingRange = 15f;
    }

    
    /*protected override void EnemyMove()
    {
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
    }

    protected override void EnemyFixedUpdate()
	{
        UpdateReachedBorder();
        if (reachedBorder) Flip();
    }*/

    protected override void Attack()
    {
        Debug.Log("Witch Attacking");
    }
}

 