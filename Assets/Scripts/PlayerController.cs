using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private Transform meleeAttackPoint;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask destructibleLayerMask;
    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip[] shotAudioClips;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float meleeDamage = 1f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;

    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private Collider2D[] results = new Collider2D[1];
    private bool jump, isGrounded, wasJumping, isAlive = true;
    private float life = 3f;
    private float shootVelocity = 5f;

    private AudioSource myAudioSource;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;
    private float jumpTimer;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        UpdateLifebarImage();
    }

    private void UpdateLifebarImage()
    {
        UIManager.Instance.UpdatePlayerLife(life);
    }

    private void Update()
    {
        jumpTimer -= Time.deltaTime;
        
        if (!GameManager.Instance.IsPaused)
        {
            if (isAlive)
            {
                if (Time.time >= nextAttackTime)
                {
                    if (Input.GetButtonDown("Fire1"))
                    // if (CrossPlatformInputManager.GetButtonDown("Fire1"))
                    {
                        // AttackMelee();
                        animator.SetTrigger("AttackMelee");
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
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

                playerRigidbody.velocity = new Vector2(horizontalInput * speed, playerRigidbody.velocity.y);

                animator.SetFloat("HorizontalSpeed", Mathf.Abs(playerRigidbody.velocity.x));
            }
        }
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            isGrounded = CheckForGround();

            if (wasJumping && isGrounded && jumpTimer <= 0)
            {
                animator.SetBool("IsJumping", false);
                wasJumping = false;
            }

            if (jump && isGrounded && jumpTimer <= 0)
            {
                wasJumping = true;
                jumpTimer = 0.1f;
                animator.SetBool("IsJumping", true);
                playerRigidbody.velocity = Vector2.up * jumpForce;

                myAudioSource.PlayOneShot(jumpAudioClip);
            }

            jump = false;
        }
    }

    private void Shoot()
    {
        animator.SetTrigger("Attack");    
    }

    private void AttackMelee()
    {
        // animator.SetTrigger("AttackMelee");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, attackRange, enemyLayerMask);
        Collider2D[] hitDesctructibles = Physics2D.OverlapCircleAll(meleeAttackPoint.position, attackRange, destructibleLayerMask);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(meleeDamage);
        }
        foreach (Collider2D desctructible in hitDesctructibles)
        {
            desctructible.GetComponent<Destructible>().Destroy();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (meleeAttackPoint == null) return;
        Gizmos.DrawWireSphere(meleeAttackPoint.position, attackRange);
    }

    private void AnimatorEventShoot()
    {
        GameObject star =
            Instantiate(starPrefab, throwPoint.position, throwPoint.rotation);
        star.GetComponent<Rigidbody2D>().velocity = throwPoint.right * shootVelocity;
        myAudioSource.PlayOneShot(shotAudioClips[Random.Range(0, shotAudioClips.Length)]);
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
        if (isAlive)
        {
            animator.SetTrigger("Hurt");
            life -= damage;
            if (life < 0f)
            {
                life = 0f;
            }

            UpdateLifebarImage();

            if (life == 0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isAlive = false;
        animator.SetBool("IsDead", true);
        //Destroy(gameObject);
    }
}
