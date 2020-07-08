using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
        private float timeBetweenSpells = 0.8f;
        protected float rangedAttackCooldown = 4f;

        private Vector3 leftThrowPoint;
        
        private int attackTimes;
        private int maxAttackTimes = 3;
        protected override void Init()
        {
            life = maxHealth = 100f;
        }
        
        protected override void Update()
        {
            if (!isAlive) return;
            UpdateDirection();
            CheckCanFlip(direction);

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
            //TODO
            //animator.SetBool(AnimIsAttacking, false);
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
            if (Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRange, results, playerLayerMask) == 0) return;
            //results[0].GetComponent<PlayerController>().TakeDamage(attackDamage);
            //audioSource.PlayOneShot(attackAudioClip); 
            UpdateAttackType();
        }
        
        private void RangedAttack()
        {
            UpdateLeftThrowPoint();
            StartCoroutine(RangedAttackAsync());
        }

        private void UpdateLeftThrowPoint()
        {
            var position = attackPoint.position;
            var dist = position.x - transform.position.x;
            leftThrowPoint = position;
            leftThrowPoint.x -= dist * 2;
        }

        //TODO: Stop coroutine when dying and when taking damage
        private IEnumerator RangedAttackAsync()
        {
            int spellsThrown = 0;
            while (spellsThrown < spellsPerAttack)
            {
                Shoot();
                yield return new WaitForSeconds(timeBetweenSpells);
                spellsThrown++;
            }
            attackMode = false;
            animator.SetBool(AnimIsAttacking, false);
            UpdateAttackType();
        }

        private void UpdateAttackType()
        {
            attackTimes++;
            if (attackTimes < maxAttackTimes) return;
            
            rangedMode = !rangedMode;
            attackTimes = 0;
            nextAttackTime = Time.time;
            attackMode = false;
            //Debug.Log("Switch to: " + rangedMode);
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
            //this isnt needed?
            //rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
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
        
        private void StartAttacking()
        {
            //Debug.Log("start attacking");
            //animator.SetTrigger(AnimAttack);
            //animator.SetBool(AnimIsAttacking, true);
            attackMode = true;

            if (rangedMode)
            {
                nextAttackTime = Time.time + rangedAttackCooldown;    
            }
            else
            {
                nextAttackTime = Time.time + attackCooldown;
            }
            //TODO ONLY FOR NOW THAT THERE ARE NO ANIMATIONS
            Attack();
        }
    }
}
