using UnityEngine;
using Pathfinding;

namespace Enemy
{
    public class EnemyAI : Enemy
    {
        //[SerializeField] private Transform target;
       
        //maybe not needed
        //[SerializeField] private Transform enemy;
        
        /*how close the enemy needs to be into a waypoint before it moves on to the next one*/
        [SerializeField] private float nextWayPointDistance = 3f;
        [SerializeField] private LayerMask playerLayerMask;

        //public AIPath aiPath;
        private Path path;
        private int currentWaypoint = 0;
        private bool reachedEndOfPath = false;

        private Seeker seeker;
        private Vector2 force;
        
        protected override void Attack()
        {
            if (Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRange, results, playerLayerMask) == 0) return;
            results[0].GetComponent<PlayerController>().TakeDamage(attackDamage);
            audioSource.PlayOneShot(attackAudioClip);        }

        protected override void Init()
        {
            //throw new System.NotImplementedException();
        }

        protected override void Start()
        {
            //same on the non AI
            int index = SpriteRenderingOrderManager.Instance.GetEnemyOrderInLayer();
            GetComponent<SpriteRenderer>().sortingOrder = index;
            UpdateLifebar();

            //Debug.Log("AI Started");
            seeker = GetComponent<Seeker>();
            //rb = GetComponent<Rigidbody2D>();

            /*generate a path*/
            InvokeRepeating(nameof(UpdatePath), 0f, .5f);
            UpdatePath();
        }

        protected override void Update()
        {
            if (!isAlive) return;
            
            /*It doesn't attack when the player is on other platform*/
            if (PlayerOnAttackRange())
            {
                if (Time.time >= nextAttackTime && isAlive) StartAttacking();
                //attackMode = true;
                return;
            }
            /*So it doesn't move while still attacking*/
            if (Time.time < nextAttackTime) return;
            animator.SetBool(AnimIsAttacking, false);
            //attackMode = false;
        }

        private void UpdatePath()
        {
            if (seeker.IsDone())
            {
                //seeker.StartPath(rigidBody.position, target.position, OnPathComplete);
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

            /*if reached the end of the path*/
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }

            reachedEndOfPath = false;
            Vector2 direction = ((Vector2) path.vectorPath[currentWaypoint] - rigidBody.position).normalized;
            //Vector2 force = direction * (speed * Time.deltaTime);
            force = direction * (speed * Time.deltaTime);
            rigidBody.AddForce(force);

            float distance = Vector2.Distance(rigidBody.position, path.vectorPath[currentWaypoint]);
            /*It reached the waypoint*/
            if (distance < nextWayPointDistance)
            {
                currentWaypoint++;
            }

            /*Flipping*/
            if (force.x >= 0.01f)
            {
                //enemy.localScale = new Vector3(-1f, 1f, 1f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (force.x <= -0.01f)
            {
                //enemy.localScale = new Vector3(1f, 1f, 1f);
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        protected override bool PlayerOnAttackRange()
        {
            return Vector2.Distance(rigidBody.position, player.position) <= attackRange;
        }

        protected override bool PlayerOnSensingRange()
        {
            //var distance = Vector2.Distance(rigidBody.position, player.position);
            //Debug.Log("Distance: " + distance);
            return Vector2.Distance(rigidBody.position, player.position) <= sensingRange;
        }
    }
}
