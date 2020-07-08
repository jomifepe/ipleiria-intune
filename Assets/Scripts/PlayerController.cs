using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private Transform meleeAttackPoint;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask destructibleLayerMask;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip[] meleeAudioClips;
    [SerializeField] private AudioClip rangedAudioClip;
    [SerializeField] private float meleeAttackRange = 0.5f;
    [SerializeField] private float meleeDamage = 1f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private Collider2D[] results = new Collider2D[1];

    private float horizontalInput;
    private bool jump, isGrounded, wasJumping, isAlive = true;
    private float health = 3f, throws = 3f, maxHealth, maxThrows;
    private float shootVelocity = 5f, attackRate = 2f;
    private float nextMeleeAttackTime, nextRangedAttackTime;
    private int extraJumps, maxJumps = 2;
    private Buff currentBuff = Buff.None;
    private Dictionary<Buff, (Sprite sprite, RuntimeAnimatorController animation)> buffResources;

    private static readonly int AnimHurt = Animator.StringToHash("Hurt");
    private static readonly int AnimIsDead = Animator.StringToHash("IsDead");
    private static readonly int AnimIsJumping = Animator.StringToHash("IsJumping");
    private static readonly int AnimHorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
    private static readonly int AnimAttackMelee = Animator.StringToHash("AttackMelee");
    private static readonly int AnimDeath = Animator.StringToHash("Death");
    private static readonly int AnimIsAttackingMelee = Animator.StringToHash("IsAttackingMelee");
    private static readonly int AnimJump = Animator.StringToHash("Jump");

    #region Lifecycle
    private void Awake()
    {
        maxHealth = health;
        maxThrows = throws;
        extraJumps = maxJumps;
        GameManager.Instance.SetPlayerMaxHealth(maxHealth);
        GameManager.Instance.SetPlayerMaxThrows(maxThrows);
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentBuff = Buff.None;

        buffResources = new Dictionary<Buff, (Sprite sprite, RuntimeAnimatorController animation)>
        {
            {Buff.None, (
                sprite: Resources.Load<Sprite>("Sprites/Weapons/Tomahawk1"), 
                animation: Resources.Load<RuntimeAnimatorController>("Animations/Tomahawk1")
            )},
            {Buff.Slow, (
                sprite: Resources.Load<Sprite>("Sprites/Weapons/Tomahawk1Icy"),
                animation: Resources.Load<RuntimeAnimatorController>("Animations/Tomahawk1Icy")
            )},
            {Buff.Physical, (
                sprite: Resources.Load<Sprite>("Sprites/Weapons/Tomahawk1Bloody"),
                animation: Resources.Load<RuntimeAnimatorController>("Animations/Tomahawk1Bloody")
            )}
        };
    }

    private void Start()
    {
        UpdateHealthBar();
        UpdateThrowBar();
    }

    private void Update()
    {
        if (!GameManager.Instance.IsPaused && isAlive)
        {
            /* melee attack */
            if (Time.time >= nextMeleeAttackTime && (
                Input.GetButtonDown("Fire1") || CrossPlatformInputManager.GetButtonDown("Fire1")
            )) PerformAttackAnimation();
            
            /* projectile throw */
            if (Time.time >= nextRangedAttackTime && (
                Input.GetButtonDown("Fire2") || CrossPlatformInputManager.GetButtonDown("Fire2")
            )) Throw();

            /* horizontal movement */
            horizontalInput = Input.GetAxisRaw("Horizontal") + CrossPlatformInputManager.GetAxisRaw("Horizontal");

            /* character horizontal flip */
            if (transform.right.x * horizontalInput < 0) Flip();

            /* jumping */
            if (!jump && (Input.GetButtonDown("Jump") || CrossPlatformInputManager.GetButtonDown("Jump"))) jump = true;

            /* check if the song/buff has changed */
            var currentSong = GameManager.Instance.CurrentSong;
            if (currentSong != null && currentSong.buff != currentBuff) currentBuff = currentSong.buff;
        }
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            /* horizontal movement */
            var velocity = rigidBody.velocity;
            rigidBody.velocity = new Vector2(horizontalInput * speed, velocity.y);
            animator.SetFloat(AnimHorizontalSpeed, Mathf.Abs(velocity.x));
            
            /* jumping */
            isGrounded = CheckForGround();
            if (isGrounded && wasJumping)
            {
                wasJumping = false;
                animator.SetBool(AnimIsJumping, false);
                extraJumps = maxJumps;
            }

            if (jump && extraJumps > 0)
            {
                extraJumps--;
                PerformJump();
            } else if (jump && extraJumps == 0 && isGrounded)
            {
                PerformJump();
            }

            jump = false;
        }
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Platform")) return;
        var bounds = other.collider.bounds;
        GameManager.Instance.platformBounds = (bounds.min.x, bounds.max.x);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null) return;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
    }

    private void PerformJump()
    {
        wasJumping = true;
        animator.SetTrigger(AnimJump);
        rigidBody.velocity = Vector2.up * jumpForce;
        audioSource.PlayOneShot(jumpAudioClip);
    }

    private void PerformAttackAnimation()
    {
        animator.SetBool(AnimIsAttackingMelee, true);
        animator.SetTrigger(AnimAttackMelee);
        Debug.Log("Attack started");
            nextMeleeAttackTime = Time.time + 1f / attackRate;
        audioSource.PlayOneShot(meleeAudioClips[0]);
    }
    
    /* Called by animation event */
    private void AttackMelee()
    {
        CameraFollow.Instance.Shake(.2f, 0.1f);
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, enemyLayerMask);
        Collider2D[] hitDestructibles = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRange, destructibleLayerMask);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy.Enemy>().TakeDamage(meleeDamage);
        }
        foreach (Collider2D destructible in hitDestructibles)
        {
            destructible.GetComponent<Destructible>().Destroy();
        }
    }
    
    private void Throw()
    {
        if (throws.Equals(0f)) return;
        // animator.SetTrigger("Throw"); // TODO: Throw animation
        AnimatorEventThrow();
        DecrementThrows();
        nextRangedAttackTime = Time.time + 1f / attackRate;
    }

    private void DecrementThrows()
    {
        throws -= 1;
        if (throws < 0) throws = 0;
        UpdateThrowBar();
    }
    
    private void AnimatorEventThrow()
    {
        GameObject throwable = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);
        throwable.GetComponent<Projectile>().buff = currentBuff;
        throwable.GetComponent<Rigidbody2D>().velocity = throwPoint.right * shootVelocity;
        throwable.GetComponent<SpriteRenderer>().sprite = buffResources[currentBuff].sprite;
        throwable.GetComponent<Animator>().runtimeAnimatorController = buffResources[currentBuff].animation;

        audioSource.PlayOneShot(rangedAudioClip);
    }
    
    private bool CheckForGround()
    {
        return Physics2D.OverlapPointNonAlloc(groundCheck.position, results, groundLayerMask) > 0;
    }

    private void Flip()
    {
        Vector3 localRotation = transform.localEulerAngles;
        localRotation.y += 180f;
        transform.localEulerAngles = localRotation;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        animator.SetTrigger(AnimHurt);
        health -= damage;
        if (health < 0f) health = 0f;
        UpdateHealthBar();
        if (health == 0f) Die();
    }

    private void Die()
    {
        isAlive = false;
        rigidBody.velocity = Vector2.zero;
        animator.SetTrigger(AnimDeath);
        animator.SetBool(AnimIsDead, true);
    }
    
    public void Kill()
    {
        TakeDamage(health);
    }

    public bool IsFullHealth()
    {
        return health.Equals(maxHealth);
    }
    
    public bool HasAllThrows()
    {
        return throws.Equals(maxThrows);
    }
    
    public void GiveHealth(float value)
    {
        health += value;
        if (health > maxHealth) health = maxHealth;
        UpdateHealthBar();
    }
    
    public void GiveThrow(float value)
    {
        throws += value;
        if (throws > maxThrows) throws = maxThrows;
        UpdateThrowBar();
    }
    
    private void UpdateHealthBar()
    {
        GameManager.Instance.UpdatePlayerLife(health);
    }
    
    private void UpdateThrowBar()
    {
        GameManager.Instance.UpdatePlayerThrows(throws);
    }

    public void SetAttackEnded()
    {
        animator.SetBool(AnimIsAttackingMelee, false);
        Debug.Log("Attack Ended");
    }
}
