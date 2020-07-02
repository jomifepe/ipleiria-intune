using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject starPrefab;

    [SerializeField] private Transform throwPoint;
    [SerializeField] private Transform meleeAttackPoint;

    private Rigidbody2D playerRigidbody;
    private Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask enemyLayerMask;

    private Collider2D[] results = new Collider2D[1];

    [SerializeField] private float speed = 3.5f;
    [SerializeField] private float jumpForce = 5f;

    private bool jump = false;
    private bool isGrounded = false;
    private float life = 100f;
    private bool isAlive = true;
    private float shootVelocity = 5f;

    private AudioSource myAudioSource;

    [SerializeField] private AudioClip jumpAudioClip;
    [SerializeField] private AudioClip[] shotAudioClips;

    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float meleeDamage = 1f;
    private float attackRate = 2f;
    private float nextAttackTime = 0f;

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
        if (!GameManager.Instance.IsPaused)
        {
            if (isAlive)
            {
                if (Time.time >= nextAttackTime)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        AttackMelee();
                        nextAttackTime = Time.time + 1f / attackRate;
                    }
                }

                float horizontalInput = Input.GetAxisRaw("Horizontal");

                if (transform.right.x > 0 && horizontalInput < 0)
                {
                    Flip();
                }

                if (transform.right.x < 0 && horizontalInput > 0)
                {
                    Flip();
                }

                if (Input.GetButtonDown("Jump"))
                {
                    jump = true;
                }

                playerRigidbody.velocity =
                new Vector2(horizontalInput * speed, playerRigidbody.velocity.y);

                animator.SetFloat("HorizontalSpeed", Mathf.Abs(playerRigidbody.velocity.x));
            }
        }
    }

    private void Shoot()
    {
        animator.SetTrigger("Attack");    
    }

    private void AttackMelee()
    {
        animator.SetTrigger("AttackMelee");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, attackRange, enemyLayerMask);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(meleeDamage);
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

    private void FixedUpdate()
    {
        if (isAlive)
        {
            isGrounded = CheckForGround();

            if (jump && isGrounded)
            {
                playerRigidbody.velocity = Vector2.zero;
                playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                myAudioSource.PlayOneShot(jumpAudioClip);
            }

            jump = false;
        }
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
        //TODO Trigger hurt animation
        life -= damage;
        if (life < 0f) life = 0f;
        UpdateLifebarImage();
        if (life == 0f) Die();     
    }

    private void Die()
    {
        isAlive = false;
        animator.SetTrigger("Die");
        //Destroy(gameObject);
    }
}
