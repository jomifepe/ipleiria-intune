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
    [SerializeField] private GameObject coinPrefab;

    private Camera mainCamera;
    private Random rng;

    protected abstract float getMaxHealth();
    protected abstract int getMinCoinDrop();
    protected abstract int getMaxCoinDrop();
    protected abstract int getMaxCoinCount();
    protected abstract int getMinCoinCount();
    
    private void Awake()
    {
        Life = getMaxHealth();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = FindObjectOfType<Camera>();
        rng = new Random();
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

            if (Life < 0f) Life = 0f;
            UpdateLifebarImage();

            if (Life == 0f) Die();
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
        DropCoins();
        Invoke(nameof(DestroyEnemy), 3);
    }

    private void DropCoins()
    {
        var tr = transform;
        for (int i = 0; i < getMaxCoinCount(); i++)
        {
            float posX = (float) (rng.NextDouble() * 1f);
            if (rng.Next(0, 2) == 1) posX *= -1f;
            Vector3 position = new Vector3(tr.position.x - posX, tr.position.y);
            GameObject spawnedObject = Instantiate(coinPrefab, position, tr.rotation);
            Coin coin = spawnedObject.GetComponent<Coin>();
            coin.setCoinValue(rng.Next(getMinCoinDrop(), getMaxCoinDrop() + 1));
        }
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
