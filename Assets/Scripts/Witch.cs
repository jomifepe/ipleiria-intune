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

    /* protected override void enemyUpdate()
     {
         rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);

         Vector3 direction = player.position - transform.position;
         //posso voltar a por como estava maybe
         updateCanFlip(direction);//change method name
         //sensingRange está dentro do sensingRange range, entao paara
         if (Mathf.Abs(direction.x) > SensingRange)
         {
             movement = Vector2.zero;
             InRange = false;
             return;
         }
         InRange = true;

         //Debug.Log(transform.localEulerAngles.y);
         //Debug.Log(direction.x);

         direction.Normalize();
         //so he doesn't jump
         direction.y = 0f;
         movement = direction;
     }*/

    /*protected override void EnemyMove()
    {
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);

        Vector3 direction = player.position - transform.position;
        //posso voltar a por como estava maybe
        UpdateCanFlip(direction);//change method name
        //sensingRange está dentro do sensingRange range, entao paara
        if (Mathf.Abs(direction.x) > sensingRange)
        {
            movement = Vector2.zero;
            InRange = false;
            return;
        }
        InRange = true;

        //Debug.Log(transform.localEulerAngles.y);
        //Debug.Log(direction.x);

        direction.Normalize();
        //so he doesn't jump
        direction.y = 0f;
        movement = direction;
   }*/

    protected override void EnemyMove()
    {
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
    }

    protected override void EnemyFixedUpdate()
	{
        UpdateReachedBorder();
        if (reachedBorder) Flip();
    }

    protected override void Attack()
    {
        
    }
}

 