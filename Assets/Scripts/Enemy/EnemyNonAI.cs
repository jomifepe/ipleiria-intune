using UnityEngine;

namespace Enemy
{
    public abstract class EnemyNonAI : Enemy
    {
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayerMask;
        private bool reachedBorder;
        private bool samePlatform;

        protected enum MovementType{SimpleMove, FollowPlayer, FollowPlayerSmart}
        [SerializeField] protected MovementType movementType;

        private const float YDiffTreshold = .5f;

        protected override void Update()
        {
            if (!isAlive) return;
            UpdateDirection();
            CheckCanFlip(direction);
            CheckSamePlatform();
        
            /*It doesn't attack when the player is on other platform*/
            if (PlayerOnAttackRange() && samePlatform)
            {
                if (canFlip && movementType == MovementType.FollowPlayerSmart) Flip();
                if (Time.time >= nextAttackTime) StartAttacking();
                attackMode = true;
                return;
            }
            /*So it doesn't move while still attacking*/
            if (Time.time < nextAttackTime) return;
            animator.SetBool(AnimIsAttacking, false);
            attackMode = false;
            Move();
        }
        
        protected override void FixedUpdate()
        {
            if (attackMode || !isAlive) return;
            CheckReachedBorder();
            if (movementType == MovementType.SimpleMove)
            {
                MoveNormally();
                return;
            }
            FollowPlayer();
        }
        
        private void Move()
        {
            rigidBody.velocity = new Vector2(speed * transform.right.x, rigidBody.velocity.y);
            if (movementType == MovementType.SimpleMove) return;
            if (!PlayerOnSensingRange())
            {
                inSensingRange = false;
                return;
            }

            if (!samePlatform) return;
            inSensingRange = true;
            UpdateMovement(direction);
        }

        private void CheckReachedBorder()
        {
            Collider2D[] hitPlatforms = Physics2D.OverlapPointAll(groundCheck.position, groundLayerMask);
            var size = hitPlatforms.Length;
            if (size != 1)
            {
                reachedBorder = true;
                return;
            }
            var position = transform.position;
            reachedBorder = !(hitPlatforms[0].bounds.min.x <= position.x &&
                              hitPlatforms[0].bounds.max.x >= position.x);
        }
        
        private void FollowPlayer()
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

        private void CheckSamePlatform()
        {
            samePlatform = SamePlatform();
            if(!samePlatform) return;

            CheckReachedBorder();
            if (!reachedBorder || !SameDirection(direction)) return;
            Flip();
        }
        
        private bool SamePlatform()
        {
            var position = transform.position;
            var enemyXPosition = position.x;
            var (leftLimit, rightLimit) = GameManager.Instance.platformBounds;
            var distanceY = Mathf.Abs(position.y - player.position.y);
            return enemyXPosition >= leftLimit && enemyXPosition <= rightLimit && distanceY < YDiffTreshold;
        }

        protected override bool PlayerOnAttackRange()
        {
            /*If not FollowPlayerSmart the player has to be in front of the enemy*/
            if (movementType != MovementType.FollowPlayerSmart && !SameDirection(direction)) return false;
            return Mathf.Abs(direction.x) <= attackRange;
        }
        
        protected override bool PlayerOnSensingRange()
        {
            return Mathf.Abs(direction.x) <= sensingRange;
        }
    }
}