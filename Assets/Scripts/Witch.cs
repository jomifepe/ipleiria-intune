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
        Life = maxHealth = 3f;
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

 