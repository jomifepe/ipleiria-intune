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
    protected float maxHealth;
    protected bool IsAlive = true;
    private bool canFlip;
    protected bool inRange = false;
    protected bool reachedBorder;
    protected float sensingRange;
    protected Vector3 direction;
    private LootDropper coinDropper;

    protected bool attackMode;
    private float attackRate = 2f;
    private float nextAttackTime;
    
    protected bool diffPlatforms;
    private float triggerPosition = -1f;
    private bool right = true;

    public Transform player;
    
    private string AnimAttack = "Attack";
    private string AnimIsAttacking = "IsAttacking";
    private string AnimHurt = "Hurt";
    private string AnimIsDead = "IsDead";

    protected abstract void Attack();
    protected abstract void Init();
    protected abstract void EnemyMove();
    protected abstract void EnemyFixedUpdate();
    
    private void Awake()
    {
        Init();
        UpdateDiffPlatforms();
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
        Debug.Log("Player position: " + player.position.x);
        if (!IsAlive) return;
        direction = player.position - transform.position;

        UpdateCanFlip(direction);

        //so it doesn't attack when on another platform
        UpdateDiffPlatforms();
        if (PlayerOnAttackRange(direction.x) && !diffPlatforms)
        {
            //try to put this flip only on one side
            if (canFlip) Flip();
            if (Time.time >= nextAttackTime && IsAlive)
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
        if (reachedBorder) Flip();
    }
    protected void FollowPlayer()
    {
        if (canFlip) Flip();
        MoveCharacter(movement);
    }

    private bool PlayerOnAttackRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= attackRange;
    }

    protected bool PlayerOnSensingRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= sensingRange;
    }

    private void MoveCharacter(Vector2 direction)
    {
        rigidBody.MovePosition((Vector2)transform.position + (direction * (speed * Time.deltaTime)));
    }

    protected void UpdateReachedBorder()
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

    //todo delete this funtion and use this one
    private bool SameDirection(Vector2 direction)
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
        animator.SetTrigger(AnimHurt);
        Life -= damage;
        if (Life < 0f) Life = 0f;
        UpdateLifebar();
        if (Life == 0f) Die();
    }

    private void UpdateLifebar()
    {
        lifebarImage.fillAmount = Life / maxHealth;
    }

    private void Die()
    {
        IsAlive = false;
        animator.SetBool(AnimIsDead, true);
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
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
