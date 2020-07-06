using System;
using JetBrains.Annotations;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        protected Rigidbody2D rigidBody;
        
        //maybe only used on the non ai
        //protected Vector2 movement;

        protected readonly Collider2D[] results = new Collider2D[1];
        private Camera mainCamera;
        
        [SerializeField] protected float speed = 1f;
    
        //maybe only used on the non ai
        protected bool inSensingRange;
        protected float sensingRange;

        //maybe only used on the non ai
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
        [SerializeField] private Canvas lifebarCanvas;
        protected bool isAlive = true;
        protected float life;
        protected float maxHealth;
        #endregion
    
        [UsedImplicitly] 
        protected abstract void Attack();
        protected abstract void Init();

        private void Awake()
        {
            Init();
            player = GameObject.Find("Player").transform;
            rigidBody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            mainCamera = FindObjectOfType<Camera>();
            audioSource = GetComponent<AudioSource>();
        }

        [UsedImplicitly]
        protected abstract void Start();
        /*private void Start()
        {
            int index = SpriteRenderingOrderManager.Instance.GetEnemyOrderInLayer();
            GetComponent<SpriteRenderer>().sortingOrder = index;
            UpdateLifebar();
        }*/

        [UsedImplicitly]
        protected abstract void Update();
        /*protected void Update()
        {
            if (!isAlive) return;
            UpdateDirection();
            CheckCanFlip(direction);
            CheckSamePlatform();
        
            /*It doesn't attack when the player is on other platform
            if (PlayerOnAttackRange(direction.x) && samePlatform)
            {
                if (canFlip && movementType == MovementType.FollowPlayerSmart) Flip();
                if (Time.time >= nextAttackTime && isAlive) StartAttacking();
                attackMode = true;
                return;
            }
            /*So it doesn't move while still attacking
            if (Time.time < nextAttackTime) return;
            animator.SetBool(AnimIsAttacking, false);
            attackMode = false;
            Move();
        }*/

        [UsedImplicitly]
        protected abstract void FixedUpdate();
        
        /*private void FixedUpdate()
        {
            if (attackMode || !isAlive) return;
            CheckReachedBorder();
            if (movementType == MovementType.SimpleMove)
            {
                MoveNormally();
                return;
            }
            FollowPlayer();
        }*/
    
        /*private void FollowPlayer()
        {
            if (!SamePlatform() || !inSensingRange)
            {
                MoveNormally();
                return;
            }
            if (reachedBorder) return;
            if (canFlip) Flip();
            MoveCharacter(movement);
        }

        private void MoveNormally()
        {
            if (reachedBorder) Flip();
        }

        protected void CheckSamePlatform()
        {
            samePlatform = SamePlatform();
            if(!samePlatform) return;
        
            CheckReachedBorder();
            if (!reachedBorder || !SameDirection(direction)) return;
            Flip();
        }*/
/*
        private bool SamePlatform()
        {
            var enemyXPosition = transform.position.x;
            var (leftLimit, rightLimit) = GameManager.Instance.platformBounds;
            return enemyXPosition >= leftLimit && enemyXPosition <= rightLimit;
        }*/

        /*private void Move()
        {
            rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
            if (movementType == MovementType.SimpleMove) return;
            if (!PlayerOnSensingRange(direction.x))
            {
                inSensingRange = false;
                return;
            }
        
            if (!samePlatform) return;
            inSensingRange = true;
            UpdateMovement(direction);
        }*/

        /*private void UpdateDirection()
        {
            direction = player.position - transform.position;
        }*/

        
        //overriden by the 2
        [UsedImplicitly] 
        protected abstract bool PlayerOnAttackRange();
        /*protected bool PlayerOnAttackRange(float playerDistanceX)
        {
            /*If not FollowPlayerSmart the player has to be in front of the enemy
            if (movementType != MovementType.FollowPlayerSmart && !SameDirection(direction)) return false;
            return Mathf.Abs(playerDistanceX) <= attackRange;
        }*/

        
        //can be oerriden for the 2 classes
        [UsedImplicitly]
        protected abstract bool PlayerOnSensingRange();
        /*protected bool PlayerOnSensingRange(float playerDistanceX)
        {
            return Mathf.Abs(playerDistanceX) <= sensingRange;
        }*/

        /*private void MoveCharacter(Vector2 dir)
        {
            rigidBody.MovePosition((Vector2)transform.position + (dir * (speed * Time.deltaTime)));
        }*/
    
        /*private void UpdateMovement(Vector2 newMov)
        {
            newMov.Normalize();
           //so he doesn't jump
            newMov.y = 0f; 
            movement = newMov;
        }*/

        /*private void CheckReachedBorder()
        {
            reachedBorder = Physics2D.OverlapPointNonAlloc(
                groundCheck.position,
                results,
                groundLayerMask) == 0;
        }*/


    
        //overriden by th 2
        protected bool SameDirection(Vector2 dir)
        {
            var localEulerAngles = transform.localEulerAngles;
            return !(dir.x < 0 && localEulerAngles.y < 180f ||
                     dir.x > 0 && localEulerAngles.y >= 180f);
        }

        //overriden by the 2
        protected void Flip()
        {
            Vector3 localRotation = transform.localEulerAngles;
            localRotation.y += 180f;
            transform.localEulerAngles = localRotation;
            lifebarCanvas.transform.forward = mainCamera.transform.forward;
        }

        protected void StartAttacking()
        {
            //Debug.Log("start attackig");
            animator.SetTrigger(AnimAttack);
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
            if (life == 0f) Die();
            return true;
        }

        protected void UpdateLifebar()
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
        }

        private void DestroyEnemy()
        {
            Destroy(gameObject);
        }
    }
}
