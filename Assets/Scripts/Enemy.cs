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

    //atack
    protected float atackRange;
    protected float cooldown;

    [SerializeField] private Image lifebarImage;
    [SerializeField] private Canvas lifebarCanvas;

    private Camera mainCamera; 

    protected abstract float getMaxHealth();
    protected abstract float getSensingRange();
    protected abstract void enemyUpdate();
    protected abstract void enemyFixedUpdate();

    public Transform player;

    private void Awake()
    {
        Life = getMaxHealth();
        SensingRange = getSensingRange();
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
        enemyUpdate();
    }

    private void FixedUpdate()
    {
        if (!IsAlive) return;
        enemyFixedUpdate();
    }

    protected void moveNormally()
    {
        updateReachedBorder();
        Debug.Log("Movign normaly");
        if (ReachedBorder) Flip();
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
        EnemyManager.Instance.DecreaseEnemiesCounter();
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
    }

    private void DestroyEnemy() //called by animation event
    {
        Destroy(gameObject);
    }

    protected void updateMovement(Vector2 mov)
    {
        mov.Normalize();
        //so he doesn't jump
        mov.y = 0f;
        movement = mov;
    }
}
