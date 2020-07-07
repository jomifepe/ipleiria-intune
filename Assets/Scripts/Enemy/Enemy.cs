using System;
using System.Collections;
using JetBrains.Annotations;
using Model;
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
        protected float originalSpeed;
        
        protected Vector2 movement;
        protected Vector3 direction;

        protected bool inSensingRange;
        protected float sensingRange;

        protected bool canFlip;
        
        protected Transform player;
        protected Coroutine slowDebuffCoroutine;

        #region Attack
        [SerializeField] protected float attackCooldown; //time for cooldown between attacks
        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackDamage;
        
        [SerializeField] protected Transform attackPoint;
    
        protected bool attackMode;
        protected float attackRate = 2f;
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
        [SerializeField] private GameObject slowIconObject;
        [SerializeField] private Image lifebarFrame;
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
            originalSpeed = speed;
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
        //protected abstract void Flip();
        protected void Flip()
        {
            Vector3 localRotation = transform.localEulerAngles;
            localRotation.y += 180f;
            transform.localEulerAngles = localRotation;
            lifebarCanvas.transform.forward = mainCamera.transform.forward;
        }
        protected void StartAttacking()
        {
            animator.SetTrigger(AnimAttack);
            //TODO: Is this needed for the non AI?
            animator.SetBool(AnimIsAttacking, true);
            nextAttackTime = Time.time + attackCooldown / attackRate;
        }
    
        public bool TakeDamage(float damage, Buff attackerBuff = Buff.None)
        {
            if (!isAlive) return false;

            switch (attackerBuff)
            {
                case Buff.Physical:
                    damage += 0.5f;
                    break;
                case Buff.Slow:
                    if (slowDebuffCoroutine != null) StopCoroutine(slowDebuffCoroutine);
                    slowDebuffCoroutine = StartCoroutine(ApplySlowDebuff());
                    break;
            }

            animator.SetTrigger(AnimHurt);
            life -= damage;
            if (life < 0f) life = 0f;
            UpdateLifebar();
            Knockback();
            if (life <= 0.01f) Die();
            return true;
        }

        private IEnumerator ApplySlowDebuff()
        {
            speed = originalSpeed - .5f;
            slowIconObject.SetActive(true);
            yield return new WaitForSeconds(4);
            speed = originalSpeed;
            slowIconObject.SetActive(false);
        }

        private void Knockback()
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            if (transform.position.x < player.position.x)
            {
                pushForce.x = -pushForce.x;
                rigidBody.AddForce(pushForce);
                pushForce.x = -pushForce.x;
                return;
            }
            rigidBody.AddForce(pushForce);
        }
        
        protected void MoveCharacter(Vector2 dir)
        {
            if(canFlip) Flip();
            rigidBody.MovePosition((Vector2)transform.position + (dir * (speed * Time.deltaTime)));
        }
        
        protected void UpdateMovement(Vector2 newMov)
        {
            newMov.Normalize();
            /*So he doesn't jump*/
            newMov.y = 0f; 
            movement = newMov;
        }
        
        protected void UpdateDirection()
        {
            direction = player.position - transform.position;
        }

        private void UpdateLifebar()
        {
            lifebarImage.fillAmount = life / maxHealth;
        }

        private void Die()
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
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

        public void ApplyBuff(Buff buff)
        {
            throw new NotImplementedException();
        }
     
        protected bool SameDirection(Vector2 dir)
        {
            var localEulerAngles = transform.localEulerAngles;
            return !(dir.x < 0 && localEulerAngles.y < 180f ||
                     dir.x > 0 && localEulerAngles.y >= 180f);
        }
        
        protected void CheckCanFlip(Vector2 dir)
        {
            canFlip = !SameDirection(dir);
        }
    }
}
