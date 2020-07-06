using System;
using JetBrains.Annotations;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        protected Rigidbody2D rigidBody;
        protected readonly Collider2D[] results = new Collider2D[1];
        protected Camera mainCamera;
        protected Vector2 pushForce = Vector2.zero;

        [SerializeField] protected float speed = 1f;
    
        protected bool inSensingRange;
        protected float sensingRange;
        
        protected Transform player;

        #region Attack
        [SerializeField] private float attackCooldown; //time for cooldown between attacks
        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackDamage;
        
        //by code put htis only on enemyNonAi
        [SerializeField] protected Transform attackPoint;
    
        protected bool attackMode;
        private float attackRate = 2f;
        protected float nextAttackTime;
        #endregion
    
        #region Audio
        [SerializeField] protected AudioClip attackAudioClip;
        protected AudioSource audioSource;
        #endregion
    
        #region Animations
        protected Animator animator;
        protected static readonly int AnimAttack = Animator.StringToHash("Attack");
        protected static readonly int AnimIsAttacking = Animator.StringToHash("IsAttacking");
        protected static readonly int AnimHurt = Animator.StringToHash("Hurt");
        protected static readonly int AnimIsDead = Animator.StringToHash("IsDead");
        #endregion

        #region Life
        [SerializeField] private Image lifebarImage;
        [SerializeField] protected Canvas lifebarCanvas;
        protected bool isAlive = true;
        protected float life;
        protected float maxHealth;
        #endregion

        private void Awake()
        {
            Init();
            player = GameObject.Find("Player").transform;
            rigidBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            mainCamera = FindObjectOfType<Camera>();
            audioSource = GetComponent<AudioSource>();
        }
        
        protected void Start()
        {
            int index = SpriteRenderingOrderManager.Instance.GetEnemyOrderInLayer();
            GetComponent<SpriteRenderer>().sortingOrder = index;
            UpdateLifebar();
        }
        
        [UsedImplicitly]
        protected abstract void Init();
        
        [UsedImplicitly] 
        protected abstract void Attack();

        [UsedImplicitly]
        protected abstract void Update();
        
        [UsedImplicitly]
        protected abstract void FixedUpdate();

        [UsedImplicitly] 
        protected abstract bool PlayerOnAttackRange();

        [UsedImplicitly]
        protected abstract bool PlayerOnSensingRange();

        [UsedImplicitly]
        protected abstract void Flip();

        protected void StartAttacking()
        {
            animator.SetTrigger(AnimAttack);
            //TODO: Is this needed for the non AI?
            animator.SetBool(AnimIsAttacking, true);
            nextAttackTime = Time.time + attackCooldown / attackRate;
        }
    
        public bool TakeDamage(float damage)
        {
            if (!isAlive) return false;
            animator.SetTrigger(AnimHurt);
            life -= damage;
            if (life < 0f) life = 0f;
            UpdateLifebar();
            Knockback();
            if (life <= 0.01f) Die();
            return true;
        }

        private void Knockback()
        {
            if (transform.position.x < player.position.x)
            {
                pushForce.x = -pushForce.x;
                rigidBody.AddForce(pushForce);
                pushForce.x = -pushForce.x;
                return;
            }
            rigidBody.AddForce(pushForce);
        }

        private void UpdateLifebar()
        {
            lifebarImage.fillAmount = life / maxHealth;
        }

        private void Die()
        {
            isAlive = false;
            animator.SetBool(AnimIsDead, true);
            rigidBody.velocity = Vector2.zero;
            rigidBody.angularVelocity = 0f;
            Invoke(nameof(DestroyEnemy), 3);
            rigidBody.gravityScale = 2f;
        }

        private void DestroyEnemy()
        {
            Destroy(gameObject);
        }
    }
}
