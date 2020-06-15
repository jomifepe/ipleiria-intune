using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D myRigidbody;

    private Animator myAnimator;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayerMask;

    private Collider2D[] results = new Collider2D[1];

    [SerializeField]
    private float speed = 1f;

    private float life = 100f;

    private bool isAlive = true;

    [SerializeField]
    private Image lifebarImage;

    [SerializeField]
    private Canvas myCanvas;

    private Camera mainCamera;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
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
        if (isAlive)
        {
            myRigidbody.velocity =
                new Vector2(speed * transform.right.x,
                myRigidbody.velocity.y
                );

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
        myCanvas.transform.forward = mainCamera.transform.forward; 
    }

    public void TakeDamage(float damage)
    {
        if (isAlive)
        {
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

    private void UpdateLifebarImage()
    {
        lifebarImage.fillAmount = life / 100f;
    }

    private void Die()
    {
        myRigidbody.velocity = Vector2.zero;
        isAlive = false;
        myAnimator.SetTrigger("Die");
        EnemyManager.Instance.DecreaseEnemiesCounter();
    }

    private void DestroyEnemy() //called by animation event
    {
        Destroy(gameObject);
    }
}
