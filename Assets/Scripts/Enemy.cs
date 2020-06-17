using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;

    private Collider2D[] results = new Collider2D[1];

    [SerializeField] private float speed = 1f;

    protected float Life;
    protected bool IsAlive = true;
    [SerializeField] private Image lifebarImage;
    [SerializeField] private Canvas lifebarCanvas;

    private Camera mainCamera;

    protected abstract float getMaxHealth();
    
    private void Awake()
    {
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

    private void Update()
    {
        if (IsAlive)
        {
            rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
            //myAnimator.SetFloat("HorizontalSpeed", Mathf.Abs(myRigidbody.velocity.x));
        }
    }

    private void FixedUpdate()
    {
        if (Physics2D.OverlapPointNonAlloc(
            groundCheck.position,
            results,
            groundLayerMask) == 0)
        {
            Flip();
        }
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
        if (IsAlive)
        {
            animator.SetTrigger("Hurt");
            Life -= damage;

            if (Life < 0f)
            {
                Life = 0f;
            }

            UpdateLifebarImage();

            if (Life == 0f)
            {
                Die();
            }
        }
    }

    private void UpdateLifebarImage()
    {
        lifebarImage.fillAmount = Life / getMaxHealth();
    }

    private void Die()
    {
        IsAlive = false;
        animator.SetBool("IsDead", true);
        EnemyManager.Instance.DecreaseEnemiesCounter();
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
    }

    private void DestroyEnemy() //called by animation event
    {
        Destroy(gameObject);
    }
}
