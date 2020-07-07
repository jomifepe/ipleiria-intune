using UnityEngine;
using Pathfinding;

namespace Enemy
{
    public class EnemyAI : Enemy
    {
        /*How close the enemy needs to be into a waypoint before it moves on to the next one*/
        [SerializeField] private float nextWayPointDistance = 3f;
        [SerializeField] private LayerMask playerLayerMask;

        private Path path;
        private int currentWaypoint;
        private bool following;
        //private bool reachedEndOfPath = false;

        private Seeker seeker;
        private Vector2 force;
        
        protected override void Attack()
        {
            if (Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRange, results, playerLayerMask) == 0) return;
            results[0].GetComponent<PlayerController>().TakeDamage(attackDamage);
            audioSource.PlayOneShot(attackAudioClip);        
        }

        protected override void Init()
        {
            life = maxHealth = 2f;
            sensingRange = 6f;
            pushForce = new Vector2(300f, 150f);
        }

        private new void Start()
        {
            base.Start();
            
            seeker = GetComponent<Seeker>();
            /*Generate a path*/
            InvokeRepeating(nameof(UpdatePath), 0f, .5f);
            UpdatePath();
        }

        protected override void Update()
        {
            if (!isAlive) return;
            
            /*It doesn't attack when the player is on other platform*/
            if (PlayerOnAttackRange())
            {
                if(!attackMode) rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                if (Time.time >= nextAttackTime && isAlive) StartAttacking();
                attackMode = true;
                return;
            }
            if(attackMode) rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            /*So it doesn't move while still attacking*/
            if (Time.time < nextAttackTime) return;
            animator.SetBool(AnimIsAttacking, false);
            attackMode = false;
        }

        private void UpdatePath()
        {
            if (seeker.IsDone())
            {
                seeker.StartPath(rigidBody.position, player.position, OnPathComplete);
            }
        }

        private void OnPathComplete(Path p)
        {
            if (p.error) return;
            path = p;
            currentWaypoint = 0;
        }

        protected override void FixedUpdate()
        {
            if (path == null) return;
            if(!PlayerOnSensingRange() && !following) return;
            following = true;
            
            /*If reached the end of the path*/
            if (currentWaypoint >= path.vectorPath.Count)
            {
                //reachedEndOfPath = true;
                return;
            }

            //reachedEndOfPath = false;
            Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rigidBody.position).normalized;
            force = direction * (speed * Time.deltaTime);
            rigidBody.AddForce(force);
            
            float distance = Vector2.Distance(rigidBody.position, path.vectorPath[currentWaypoint]);
            /*It reached the waypoint*/
            if (distance < nextWayPointDistance) currentWaypoint++;
            Flip();
        }

        private new void Flip()
        {
            Vector3 localRotation = transform.localEulerAngles;
            if (force.x >= 0.01f)
            {
                localRotation.y = 0f;
            }
            else if (force.x <= -0.01f)
            {
                localRotation.y = 180f;
            }
            transform.localEulerAngles = localRotation;
            lifebarCanvas.transform.forward = mainCamera.transform.forward;
        }
        
        protected override bool PlayerOnAttackRange()
        {
            return Vector2.Distance(rigidBody.position, player.position) <= attackRange;
        }

        protected override bool PlayerOnSensingRange()
        {
            return Vector2.Distance(rigidBody.position, player.position) <= sensingRange;
        }
    }
}
