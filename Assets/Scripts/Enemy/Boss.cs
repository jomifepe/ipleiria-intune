using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Model;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Enemy
{
    public class Boss : Enemy
    {
        //Melee
        [SerializeField] private LayerMask playerLayerMask;
        //Ranged
        [SerializeField] private GameObject throwablePrefab;
        [SerializeField] private float shootVelocity = 3f;
        
        private bool rangedMode = true;
        private int spellsPerAttack = 3;
        private float timeBetweenSpells = 0.4f;
        private float rangedAttackCooldown = 4f;
        private readonly float triggerLifeValue = 0.2f;
        
        private const float RestTime = 3f;
        /*In this time the boss only walks, used in the beginning and when swapping from melee to ranged mode*/
        private float restTimer;
        private Vector3 leftThrowPoint;
        private int attackTimes;
        private int maxAttackTimes = 3;
        private Coroutine rangedAttackCoroutine;

        private static readonly int AnimIsMelee = Animator.StringToHash("Melee");

        protected override void Init()
        {
            life = maxHealth = 25f;
            UpdateDirection();
            CheckCanFlip(direction);
            if(canFlip) Flip();
            Rest();
            GameManager.Instance.InitializeBossLifebar(maxHealth);
        }
        
        protected override void Update()
        {
            if (!isAlive) return;
            UpdateDirection();
            CheckCanFlip(direction);

            if (Time.time < restTimer)
            {
                if (canFlip) Flip();
                Move();
                return;
            }
            
            if (rangedMode)
            {
                if (canFlip) Flip();
                if(Time.time >= nextAttackTime && !attackMode) StartAttacking();
                Move();
                return;
            }
            
            /*MeleeMode*/
            if (PlayerOnAttackRange())
            {
                if (canFlip) Flip();
                if(Time.time >= nextAttackTime) StartAttacking();
                return;
            }
            if(Time.time < nextAttackTime) return;
            animator.SetBool(AnimIsAttacking, false);
            attackMode = false;
            Move();
        }

        protected override void FixedUpdate()
        {
            if (!isAlive) return;
            if(attackMode) return;
            MoveCharacter(movement); 
        }
        
        protected override void Attack()
        {
            if (rangedMode)
            {
                RangedAttack();
                return;
            }
            MeleeAttack();
        }

        private void MeleeAttack()
        {
            if (Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRange, results, playerLayerMask) != 0)
            {
                results[0].GetComponent<PlayerController>().TakeDamage(attackDamage);
            }
            //TODO Sound
            //audioSource.PlayOneShot(attackAudioClip); 
            UpdateAttackType();
        }
        
        private void RangedAttack()
        {
            UpdateLeftThrowPoint();
            if (rangedAttackCoroutine != null) StopCoroutine(rangedAttackCoroutine);
            rangedAttackCoroutine = StartCoroutine(RangedAttackAsync());
        }

        private void UpdateLeftThrowPoint()
        {
            var position = attackPoint.position;
            var dist = position.x - transform.position.x;
            leftThrowPoint = position;
            leftThrowPoint.x -= dist * 2;
        }

        private IEnumerator RangedAttackAsync()
        {
            int spellsThrown = 0;
            while (spellsThrown < spellsPerAttack)
            {
                Shoot();
                yield return new WaitForSeconds(timeBetweenSpells);
                spellsThrown++;
            }
            animator.SetBool(AnimIsAttacking, false);
            attackMode = false;
            UpdateAttackType();
        }

        private void UpdateAttackType()
        {
            attackTimes++;
            if (attackTimes < maxAttackTimes) return;
            
            //Stops for a bit when transitioning from melee to ranged
            if(!rangedMode) Rest();
            rangedMode = !rangedMode;
            attackTimes = 0;
            nextAttackTime = Time.time;
            attackMode = false;
        }

        private void Shoot()
        {
            var rotation = attackPoint.rotation;
            var right = attackPoint.right;
            
            /*Right side*/
            GameObject throwable = Instantiate(throwablePrefab, attackPoint.position, rotation);
            throwable.GetComponent<Rigidbody2D>().velocity = right * shootVelocity;
            
            /*Left side*/
            GameObject throwableLeft = Instantiate(throwablePrefab, leftThrowPoint, rotation);
            /*Flipping*/
            Vector3 localRotation = throwableLeft.transform.localEulerAngles;
            localRotation.y = 180f;
            throwableLeft.transform.localEulerAngles = localRotation;
            throwableLeft.GetComponent<Rigidbody2D>().velocity = -right * shootVelocity;
        }
        
        private void Move()
        {
            if(attackMode) return;
            UpdateMovement(direction);
        }

        protected override bool PlayerOnAttackRange()
        {
            return Mathf.Abs((player.position - transform.position).x) <= attackRange;
        }

        protected override bool PlayerOnSensingRange()
        {
            return true;
        }
        
        private new void StartAttacking()
        {
            attackMode = true;
            if (rangedMode)
            {
                nextAttackTime = Time.time + rangedAttackCooldown;
                animator.SetBool(AnimIsAttacking, true);
                animator.SetBool(AnimIsMelee, false);
                Attack();
                return;
            }
            
            nextAttackTime = Time.time + attackCooldown;
            animator.SetTrigger(AnimAttack);
            animator.SetBool(AnimIsAttacking, true);
            animator.SetBool(AnimIsMelee, true);
        }

        public override bool TakeDamage(float damage, Buff attackerBuff = Buff.None)
        {
            if (life > maxHealth * triggerLifeValue && life - damage <= maxHealth * triggerLifeValue)
            {
                speed *= 1.4f;
                attackCooldown *= 0.8f;
                rangedAttackCooldown *= 0.6f;
                spellsPerAttack++;
                GameManager.Instance.UpdateBossLifeBarColor();
            }
            bool returnValue = base.TakeDamage(damage, attackerBuff);
            GameManager.Instance.UpdateBossLife(life);
            return returnValue;
        }

        private void Rest()
        {
            restTimer = Time.time + RestTime;
            animator.SetBool(AnimIsAttacking, false);
            attackMode = false;
        }
        
        [UsedImplicitly]
        protected void FinishDying()
        {
            if (rangedAttackCoroutine != null) StopCoroutine(rangedAttackCoroutine);
            GameManager.Instance.IncrementCoins(1500);
            GameManager.Instance.EndGame(true);
        }
    }
}
