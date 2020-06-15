using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject starPrefab;

    [SerializeField]
    private Transform shootPoint;

    private Rigidbody2D myRigidbody;

    private Animator myAnimator;

    [SerializeField]
    private Transform groundCheck;

    [SerializeField]
    private LayerMask groundLayerMask;

    private Collider2D[] results = new Collider2D[1];

    [SerializeField]
    private float speed = 3.5f;

    [SerializeField]
    private float jumpForce = 5f;

    private bool jump = false;

    private bool isGrounded = false;

    private float life = 100f;

    private bool isAlive = true;
    private float shootVelocity = 5f;

    private AudioSource myAudioSource;

    [SerializeField] private AudioClip jumpAudioClip;

    [SerializeField] private AudioClip[] shotAudioClips;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
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
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
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

                myRigidbody.velocity =
                new Vector2(horizontalInput * speed,
                myRigidbody.velocity.y
                );

                myAnimator.SetFloat("HorizontalSpeed", Mathf.Abs(myRigidbody.velocity.x));
            }
        }
    }

    private void Shoot()
    {
        myAnimator.SetTrigger("Attack");    
    }

    private void AnimatorEventShoot()
    {
        GameObject star =
            Instantiate(starPrefab, shootPoint.position, shootPoint.rotation);
        star.GetComponent<Rigidbody2D>().velocity = shootPoint.right * shootVelocity;
        myAudioSource.PlayOneShot(shotAudioClips[Random.Range(0, shotAudioClips.Length)]);
    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            isGrounded = CheckForGround();

            if (jump && isGrounded)
            {
                myRigidbody.velocity = Vector2.zero;
                myRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
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

    private void Die()
    {
        isAlive = false;
        myAnimator.SetTrigger("Die");
        //Destroy(gameObject);
    }
}
