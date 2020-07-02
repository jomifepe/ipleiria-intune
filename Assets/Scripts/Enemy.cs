using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    protected Rigidbody2D rigidBody;
    private Animator animator;
    protected Vector2 movement;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;

    private Collider2D[] results = new Collider2D[1];

    [SerializeField] protected float speed = 1f;

    protected float Life;
    protected bool IsAlive = true;
    protected bool CanFlip = false;
    protected bool InRange = false;
    protected bool ReachedBorder = false;
    protected float SensingRange;
    protected Vector3 direction;

    //atack
    public float timer; //time for cooldown between attacks
    public float atackRange;

    protected float cooldown;
    protected bool cooling;
    protected bool attackMode = false;
    protected bool inAttackRange;
    protected float initialTimer;

    private float attackRate = 2f;
    private float nextAttackTime = 0f;
    //atack

    [SerializeField] private Image lifebarImage;
    [SerializeField] private Canvas lifebarCanvas;

    private Camera mainCamera; 

    protected abstract float getMaxHealth();
    protected abstract float getSensingRange();
    //protected abstract void enemyUpdate();
    protected abstract void enemyMove();
    protected abstract void enemyFixedUpdate();

    public Transform player;

    private void Awake()
    {
        Life = getMaxHealth();
        SensingRange = getSensingRange();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = FindObjectOfType<Camera>();
        initialTimer = timer;
    }

    private void Start()
    {
        int index = SpriteRenderingOrderManager.Instance.GetEnemyOrderInLayer();
        GetComponent<SpriteRenderer>().sortingOrder = index;

        UpdateLifebarImage();
    }

    protected void Update()
    {
        if (!IsAlive) return;
        direction = player.position - transform.position;
        
        if (isPlayerOnAttackRange(direction.x))
        {
            if (Time.time >= nextAttackTime && IsAlive)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("IsAttacking", true);
                nextAttackTime = Time.time + 1f / attackRate;   
            }
        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }
        
        
  //       if (isPlayerOnAttackRange(direction.x))
  //       {
  //           Debug.Log("On attack range");
  //           inAttackRange = true;
  //           EnemyLogic();
		// }else
		// {
  //           Debug.Log("Not on attack range");
  //           inAttackRange = false;
  //           Move();
  //
		// 	if (attackMode)
		// 	{
  //               StopAttack();
		// 	}   
  //       }
    }

    private void EnemyLogic()
	{
        if(!cooling)
        {
            Attack();
        }else
		{
            Cooldown();
            Debug.Log("CanAttack false");
            //why it only works at the opossite way?
            animator.SetBool("CanAttack", false);
        }
    }

    private void Move()
    {
        Debug.Log("Moving");

        //TODO check if animator.SetBool("CanWalk", true) is used before or after the if
        //animator.SetBool("CanWalk", true);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAttack")) return;

        //animator.SetBool("CanWalk", true);
        //enemyMove();
    }
    //CAN BE OVERRIDEN BY WITCH
    private void Attack()
    {
        Debug.Log("Attacking");
        timer = initialTimer;
        
        attackMode = true;
        animator.SetBool("CanWalk", false);
        Debug.Log("CanAttack true");
        //why it only works at the opossite way?
        animator.SetBool("CanAttack", true);
    }

    private void StopAttack()
    {
        Debug.Log("Stoped atacking");
        cooling = false;
        attackMode = false;

        Debug.Log("CanAttack false");
        //why it only works at the opossite way?
        animator.SetBool("CanAttack", true);
    }

    public void TriggerCooling()
	{
        Debug.Log("trigger cooling");
        cooling = true;
	}

    private void Cooldown()
	{
        timer -= Time.deltaTime;
        if(timer <= 0 && cooling && attackMode)
		{
            cooling = false;
            timer = initialTimer;
		}
	}

    private void FixedUpdate()
    {
        if (!IsAlive) return;
        if (!attackMode) return;
        //enemyFixedUpdate();
    }

    protected void moveNormally()
    {
        updateReachedBorder();
        Debug.Log("Moving normaly");
        if (ReachedBorder) Flip();
    }

    protected bool isPlayerOnAttackRange(float playerDistanceX)
    {
        //Debug.Log("playerDistanceX: " + Mathf.Abs(playerDistanceX));
        //Debug.Log("atackRange: " + atackRange);
        return Mathf.Abs(playerDistanceX) <= atackRange;
    }

    protected bool isPlayerOnRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= SensingRange;
    }

    protected void moveCharacter(Vector2 direction)
    {
        rigidBody.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
    }

    protected void updateReachedBorder()
	{
        ReachedBorder = (Physics2D.OverlapPointNonAlloc(
            groundCheck.position,
            results,
            groundLayerMask) == 0);

    }

    protected void updateCanFlip(Vector2 direction)
	{
        CanFlip = (direction.x < 0 && transform.localEulerAngles.y < 180f ||
           direction.x > 0 && transform.localEulerAngles.y >= 180f);
    }

    //todo delete this funtion and use this one
    protected bool sameDirection(Vector2 direction)
    {
        return !(direction.x < 0 && transform.localEulerAngles.y < 180f ||
           direction.x > 0 && transform.localEulerAngles.y >= 180f);
    }

    protected void Flip()
    {
        Vector3 localRotation = transform.localEulerAngles;
        localRotation.y += 180f;
        transform.localEulerAngles = localRotation;
        lifebarCanvas.transform.forward = mainCamera.transform.forward;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        animator.SetTrigger("Hurt");
        Life -= damage;

        if (Life < 0f) Life = 0f;
        UpdateLifebarImage();
        if (Life == 0f)Die();
    }

    private void UpdateLifebarImage()
    {
        lifebarImage.fillAmount = Life / getMaxHealth();
    }

    private void Die()
    {
        IsAlive = false;
        animator.SetBool("IsDead", true);
        // EnemyManager.Instance.DecreaseEnemiesCounter();
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
    }

    private void DestroyEnemy() //called by animation event
    {
        Destroy(gameObject);
    }

    protected void updateMovement(Vector2 newMov)
    {
        newMov.Normalize();
        //so he doesn't jump
        newMov.y = 0f;
        movement = newMov;
    }
}
