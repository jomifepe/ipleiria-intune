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
    protected Rigidbody2D rigidBody;
    protected Vector2 movement;

    private Collider2D[] results = new Collider2D[1];
    private Camera mainCamera;
    private Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;

    [SerializeField] protected bool followPlayer;
    [SerializeField] protected float speed = 1f;

    //attack
    [SerializeField] private float cooldown; //time for cooldown between attacks
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;

    [SerializeField] private Image lifebarImage;
    [SerializeField] private Canvas lifebarCanvas;

    protected float Life;
    protected float maxLife;
    protected bool IsAlive = true;
    protected bool CanFlip = false;
    protected bool InRange = false;
    protected bool ReachedBorder = false;
    protected float sensingRange;
    protected Vector3 direction;
    private Camera mainCamera;
    private CoinDrop coinDropper;

    protected bool attackMode = false;
    protected bool inAttackRange;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;

    protected bool diffPlatforms = false;
    protected float triggerPosition = -1f;
    protected bool right = true;

    protected abstract void Attack();
    protected abstract void Init();
    protected abstract void EnemyMove();
    protected abstract void EnemyFixedUpdate();

    public Transform player;

    protected abstract float getMaxHealth();
    protected abstract int getMinCoinDrop();
    protected abstract int getMaxCoinDrop();
    protected abstract int getMaxCoinCount();
    protected abstract int getMinCoinCount();
    
    private void Awake()
    {
        Init();
        UpdateDiffPlatforms();
        coinDropper = GetComponent<CoinDrop>();
        Life = getMaxHealth();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = FindObjectOfType<Camera>();
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

        UpdateCanFlip(direction);

        //so it doesn't attack when on another platform
        UpdateDiffPlatforms();
        if (PlayerOnAttackRange(direction.x) && !diffPlatforms)
        {
            //try to put this flip only on one side
            if (CanFlip) Flip();
            if (Time.time >= nextAttackTime && IsAlive)
            {
                animator.SetTrigger("Attack");
                animator.SetBool("IsAttacking", true);
                nextAttackTime = Time.time + cooldown / attackRate;
            }
            attackMode = true;
            return;
        }
        //So it doesn't move while still atacking
        if (Time.time < nextAttackTime) return;	

        animator.SetBool("IsAttacking", false);
        attackMode = false;
        EnemyMove();  
    }


    private void FixedUpdate()
    {
        if (!IsAlive) return;
        EnemyFixedUpdate();
    }

    protected void MoveNormally()
    {
        UpdateReachedBorder();
        if (ReachedBorder) Flip();
    }

    protected bool PlayerOnAttackRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= attackRange;
    }

    protected bool PlayerOnSensingRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= sensingRange;
    }

    protected void MoveCharacter(Vector2 direction)
    {
        rigidBody.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
    }

    protected void UpdateReachedBorder()
	{
        ReachedBorder = (Physics2D.OverlapPointNonAlloc(
            groundCheck.position,
            results,
            groundLayerMask) == 0);
    }

    protected void UpdateCanFlip(Vector2 direction)
	{
        CanFlip = !SameDirection(direction);
    }

    //todo delete this funtion and use this one
    protected bool SameDirection(Vector2 direction)
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
        lifebarImage.fillAmount = Life / maxLife;
    }

    private void Die()
    {
        IsAlive = false;
        animator.SetBool("IsDead", true);
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        coinDropper.DropCoins(getMinCoinDrop(), getMaxCoinDrop(), 
            getMinCoinCount(),getMaxCoinCount());
        Invoke(nameof(DestroyEnemy), 3);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    protected void UpdateMovement(Vector2 newMov)
    {
        newMov.Normalize();
        //so he doesn't jump
        newMov.y = 0f;
        movement = newMov;
    }

    protected bool IsOnAnotherPlatform(float playerPosition)
    {
        return !((player.position.x < triggerPosition && right) ||
        (player.position.x > triggerPosition && !right));
    }

    protected void UpdateDiffPlatforms()
	{
        if (diffPlatforms)
        {
            if (IsOnAnotherPlatform(player.position.x)) return;
            diffPlatforms = false;
            return;
        }

        UpdateReachedBorder();
        if (ReachedBorder && SameDirection(direction))
        {
            triggerPosition = transform.position.x;
            right = direction.x >= 0;
            diffPlatforms = true;
            Flip();
        }
    }

    protected void FollowPlayer()
    {
        if (CanFlip) Flip();
        MoveCharacter(movement);
    }
}
