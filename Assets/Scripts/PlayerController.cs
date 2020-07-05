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

    private Rigidbody2D playerRigidbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private Collider2D[] results = new Collider2D[1];
    
    private bool jump, isGrounded, wasJumping, isAlive = true;
    private float health = 3f, throws = 3f, maxHealth, maxThrows;
    private float shootVelocity = 5f, attackRate = 2f;
    private float nextMeleeAttackTime, nextRangedAttackTime;
    private float jumpTimer;
    private Buff currentBuff;
    private Dictionary<Buff, (Sprite sprite, RuntimeAnimatorController animation)> buffResources;

    private string AnimHurt = "Hurt";
    private string AnimIsDead = "IsDead";
    private string AnimIsJumping = "IsJumping";
    private string AnimHorizontalSpeed = "HorizontalSpeed";
    private string AnimAttackMelee = "AttackMelee";


    private void Awake()
    {
        maxHealth = health;
        maxThrows = throws;
        GameManager.Instance.SetPlayerMaxHealth(maxHealth);
        GameManager.Instance.SetPlayerMaxThrows(maxThrows);
        playerRigidbody = GetComponent<Rigidbody2D>();
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
    }

    private void Update()
    {
        jumpTimer -= Time.deltaTime;
        
        if (!GameManager.Instance.IsPaused && isAlive)
        {
            if (Time.time >= nextMeleeAttackTime && Input.GetButtonDown("Fire1"))
            // if (Time.time >= nextMeleeAttackTime && CrossPlatformInputManager.GetButtonDown("Fire1"))
            {
                PerformAttackAnimation();
            }
            // if (Time.time >= nextRangedAttackTime && Input.GetButtonDown("Fire2"))
            if (Time.time >= nextRangedAttackTime && CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                Throw();
            }

            float horizontalInput = Input.GetAxisRaw("Horizontal");
            // float horizontalInput = CrossPlatformInputManager.GetAxisRaw("Horizontal");

            if (transform.right.x > 0 && horizontalInput < 0)
            {
                Flip();
            }

            if (transform.right.x < 0 && horizontalInput > 0)
            {
                Flip();
            }

            if (Input.GetButtonDown("Jump"))
            // if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                jump = true;
            }
            
            var currentSong = GameManager.Instance.CurrentSong;
            if (currentSong != null && currentSong.buff != currentBuff) currentBuff = currentSong.buff;

            playerRigidbody.velocity = new Vector2(horizontalInput * speed, playerRigidbody.velocity.y);

            animator.SetFloat(AnimHorizontalSpeed, Mathf.Abs(playerRigidbody.velocity.x));
        }
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            isGrounded = CheckForGround();

            if (wasJumping && isGrounded && jumpTimer <= 0)
            {
                animator.SetBool(AnimIsJumping, false);
                wasJumping = false;
            }

            if (jump && isGrounded && jumpTimer <= 0)
            {
                wasJumping = true;
                jumpTimer = 0.1f;
                animator.SetBool(AnimIsJumping, true);
                playerRigidbody.velocity = Vector2.up * jumpForce;

                audioSource.PlayOneShot(jumpAudioClip);
            }

            jump = false;
        }
    }

    private void PerformAttackAnimation()
    {
        animator.SetTrigger(AnimAttackMelee);
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
            enemy.GetComponent<Enemy>().TakeDamage(meleeDamage);
        }
        foreach (Collider2D desctructible in hitDestructibles)
        {
            desctructible.GetComponent<Destructible>().Destroy();
        }
    }
    
    private void Throw()
    {
        if (throws == 0f) return;
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
        throwable.GetComponent<Rigidbody2D>().velocity = throwPoint.right * shootVelocity;
        throwable.GetComponent<SpriteRenderer>().sprite = buffResources[currentBuff].sprite;
        throwable.GetComponent<Animator>().runtimeAnimatorController = buffResources[currentBuff].animation;
        audioSource.PlayOneShot(rangedAudioClip);
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null) return;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRange);
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
        animator.SetBool(AnimIsDead, true);
        //Destroy(gameObject);
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
}
