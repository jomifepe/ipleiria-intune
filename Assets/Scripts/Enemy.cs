using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = System.Random;

public abstract class Enemy : MonoBehaviour
{
    private enum MovementType{SimpleMove, FollowPlayer, FollowPlayerSmart};
    
    private Rigidbody2D rigidBody;
    private Vector2 movement;

    private Collider2D[] results = new Collider2D[1];
    private Camera mainCamera;
    private Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    
    [SerializeField] private MovementType movementType;
    [SerializeField] protected float speed = 1f;

    //attack
    [SerializeField] private float cooldown; //time for cooldown between attacks
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;

    [SerializeField] private Image lifebarImage;
    [SerializeField] private Canvas lifebarCanvas;

    protected float life;
    protected float maxHealth;
    private bool isAlive = true;
    private bool canFlip;
    private bool inSensingRange = false;
    private bool reachedBorder;
    protected float sensingRange;
    private Vector3 direction;
    private LootDropper coinDropper;

    private bool attackMode;
    private float attackRate = 2f;
    private float nextAttackTime;
    
    private bool diffPlatforms;
    private float triggerPosition = -1f;
    private bool right = true;

    private Transform player;
    
    private string AnimAttack = "Attack";
    private string AnimIsAttacking = "IsAttacking";
    private string AnimHurt = "Hurt";
    private string AnimIsDead = "IsDead";

    protected abstract void Attack();
    protected abstract void Init();
    //protected abstract void EnemyMove();
    //protected abstract void EnemyFixedUpdate();
    
    //TODO Improve samePlatform and witch attack
    //Ranged have the sensing and attack range the same
    private void Awake()
    {
        Init();
        UpdateDiffPlatforms();
        player = GameObject.Find("Player").transform;
        coinDropper = GetComponent<LootDropper>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = FindObjectOfType<Camera>();
    }

    private void Start()
    {
        int index = SpriteRenderingOrderManager.Instance.GetEnemyOrderInLayer();
        GetComponent<SpriteRenderer>().sortingOrder = index;
        UpdateLifebar();
    }

    protected void Update()
    {
        if (!isAlive) return;
        UpdateDirection();
        UpdateCanFlip(direction);
        //so it doesn't attack when the player is on other platform
        UpdateDiffPlatforms();
        //player on attack range ou analiza so a frente ou so atras
        if (PlayerOnAttackRange(direction.x) && !diffPlatforms)
        {
            //try to put this flip only on one side
            if (canFlip && movementType == MovementType.FollowPlayerSmart) Flip();
            if (Time.time >= nextAttackTime && isAlive)
            {
                animator.SetTrigger(AnimAttack);
                animator.SetBool(AnimIsAttacking, true);
                nextAttackTime = Time.time + cooldown / attackRate;
            }
            attackMode = true;
            return;
        }
        //So it doesn't move while still attacking
        if (Time.time < nextAttackTime) return;	

        animator.SetBool(AnimIsAttacking, false);
        attackMode = false;
        Move();
    }

    private void Move()
    {
        rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
        if (movementType == MovementType.SimpleMove) return;
        
        if (!PlayerOnSensingRange(direction.x))
        {
            // Debug.Log("Player isn't on sensing range");
            inSensingRange = false;
            return;
        }

        if (diffPlatforms) return;

        //Debug.Log(transform.localEulerAngles.y);
        //Debug.Log(direction.x);

        inSensingRange = true;
        UpdateMovement(direction);
    }

    private void UpdateDirection()
    {
        direction = player.position - transform.position;
    }

    private void FixedUpdate()
    {
        //EnemyFixedUpdate();
        
        if (!isAlive) return;
        if (attackMode) return;

        if (movementType == MovementType.SimpleMove)
        {
            MoveNormally();
            return;
        }
        
        //if following player
        if (diffPlatforms || !inSensingRange)
        {
            MoveNormally();
            return;
        }

        UpdateReachedBorder();
        if (!reachedBorder) FollowPlayer();
    }

    private void MoveNormally()
    {
        UpdateReachedBorder();
        if (reachedBorder) Flip();
        
    }
    private void FollowPlayer()
    {
        if (canFlip) Flip();
        MoveCharacter(movement);
    }
    
    private bool PlayerOnAttackRange(float playerDistanceX)
    {
        //If not FollowPlayerSmart the player has to be in front of the enemy
        if (movementType != MovementType.FollowPlayerSmart && !SameDirection(direction)) return false;
        return Mathf.Abs(playerDistanceX) <= attackRange;
    }

    private bool PlayerOnSensingRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= sensingRange;
    }

    private void MoveCharacter(Vector2 direction)
    {
        rigidBody.MovePosition((Vector2)transform.position + (direction * (speed * Time.deltaTime)));
    }

    private void UpdateReachedBorder()
	{
        reachedBorder = (Physics2D.OverlapPointNonAlloc(
            groundCheck.position,
            results,
            groundLayerMask) == 0);
    }

    private void UpdateCanFlip(Vector2 direction)
	{
        canFlip = !SameDirection(direction);
    }
    
    private bool SameDirection(Vector2 direction)
    {
        return !(direction.x < 0 && transform.localEulerAngles.y < 180f ||
           direction.x > 0 && transform.localEulerAngles.y >= 180f);
    }

    private void Flip()
    {
        Vector3 localRotation = transform.localEulerAngles;
        localRotation.y += 180f;
        transform.localEulerAngles = localRotation;
        lifebarCanvas.transform.forward = mainCamera.transform.forward;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        animator.SetTrigger(AnimHurt);
        life -= damage;
        if (life < 0f) life = 0f;
        UpdateLifebar();
        if (life == 0f) Die();
    }

    private void UpdateLifebar()
    {
        lifebarImage.fillAmount = life / maxHealth;
    }

    private void Die()
    {
        isAlive = false;
        animator.SetBool(AnimIsDead, true);
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        Invoke(nameof(DestroyEnemy), 3);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void UpdateMovement(Vector2 newMov)
    {
        newMov.Normalize();
        //so he doesn't jump
        newMov.y = 0f;
        movement = newMov;
    }

    private bool IsOnAnotherPlatform(float playerPosition)
    {
        return !((player.position.x < triggerPosition && right) ||
        (player.position.x > triggerPosition && !right));
    }

    private void UpdateDiffPlatforms()
	{
        if (diffPlatforms)
        {
            if (IsOnAnotherPlatform(player.position.x)) return;
            diffPlatforms = false;
            return;
        }

        UpdateReachedBorder();
        if (!reachedBorder || !SameDirection(direction)) return;
        triggerPosition = transform.position.x;
        right = direction.x >= 0;
        diffPlatforms = true;
        Flip();
    }
}
