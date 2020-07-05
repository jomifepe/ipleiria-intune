using JetBrains.Annotations;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public abstract class Enemy : MonoBehaviour
{
    private enum MovementType{SimpleMove, FollowPlayer, FollowPlayerSmart};
    
    private Rigidbody2D rigidBody;
    private Vector2 movement;

    protected readonly Collider2D[] results = new Collider2D[1];
    private Camera mainCamera;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayerMask;
    
    [SerializeField] private MovementType movementType;
    [SerializeField] protected float speed = 1f;
    
    private bool canFlip;
    private bool reachedBorder;
    private bool samePlatform;
    private bool inSensingRange;
    protected float sensingRange;

    private Vector3 direction;
    private Transform player;

    #region Attack
    [SerializeField] private float attackCooldown; //time for cooldown between attacks
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected Transform attackPoint;
    
    private bool attackMode;
    private float attackRate = 2f;
    private float nextAttackTime;
    #endregion
    
    #region Audio
    [SerializeField] protected AudioClip attackAudioClip;
    protected AudioSource audioSource;
    #endregion
    
    #region Animations
    private Animator animator;
    private static readonly int AnimAttack = Animator.StringToHash("Attack");
    private static readonly int AnimIsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int AnimHurt = Animator.StringToHash("Hurt");
    private static readonly int AnimIsDead = Animator.StringToHash("IsDead");
    #endregion

    #region Life
    [SerializeField] private Image lifebarImage;
    [SerializeField] private Canvas lifebarCanvas;
    private bool isAlive = true;
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

    private void Start()
    {
        int index = SpriteRenderingOrderManager.Instance.GetEnemyOrderInLayer();
        GetComponent<SpriteRenderer>().sortingOrder = index;
        UpdateLifebar();
    }

    protected void Update()
    {
        if (!isAlive) return;
        UpdateDirection();
        UpdateCanFlip(direction);
        UpdateSamePlatform();
        
        //it doesn't attack when the player is on other platform
        if (PlayerOnAttackRange(direction.x) && samePlatform)
        {
            if (canFlip && movementType == MovementType.FollowPlayerSmart) Flip();
            if (Time.time >= nextAttackTime && isAlive) StartAttacking();
            attackMode = true;
            return;
        }
        //So it doesn't move while still attacking
        if (Time.time < nextAttackTime) return;
        animator.SetBool(AnimIsAttacking, false);
        attackMode = false;
        Move();
    }

    private void UpdateSamePlatform()
    {
        samePlatform = SamePlatform();
        if(!samePlatform) return;
        
        UpdateReachedBorder();
        if (!reachedBorder || !SameDirection(direction)) return;
        Flip();
    }

    private bool SamePlatform()
    {
        var enemyPosition = transform.position;
        var (leftLimit, rightLimit) = GameManager.Instance.platformBounds;
        return enemyPosition.x >= leftLimit && enemyPosition.x <= rightLimit;
    }

    private void StartAttacking()
    {
        animator.SetTrigger(AnimAttack);
        animator.SetBool(AnimIsAttacking, true);
        nextAttackTime = Time.time + attackCooldown / attackRate;
    }

    private void Move()
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
    }

    private void UpdateDirection()
    {
        direction = player.position - transform.position;
    }

    private void FixedUpdate()
    {
        if (!isAlive) return;
        if (attackMode) return;

        if (movementType == MovementType.SimpleMove)
        {
            MoveNormally();
            return;
        }
        
        //if following player
        if (!SamePlatform() || !inSensingRange)
        {
            MoveNormally();
            return;
        }

        UpdateReachedBorder();
        if (!reachedBorder) FollowPlayer();
    }

    private void MoveNormally()
    {
        UpdateReachedBorder();
        if (reachedBorder) Flip();
    }
    private void FollowPlayer()
    {
        if (canFlip) Flip();
        MoveCharacter(movement);
    }
    
    private bool PlayerOnAttackRange(float playerDistanceX)
    {
        //If not FollowPlayerSmart the player has to be in front of the enemy
        if (movementType != MovementType.FollowPlayerSmart && !SameDirection(direction)) return false;
        return Mathf.Abs(playerDistanceX) <= attackRange;
    }

    private bool PlayerOnSensingRange(float playerDistanceX)
    {
        return Mathf.Abs(playerDistanceX) <= sensingRange;
    }

    private void MoveCharacter(Vector2 dir)
    {
        rigidBody.MovePosition((Vector2)transform.position + (dir * (speed * Time.deltaTime)));
    }

    private void UpdateReachedBorder()
	{
        reachedBorder = Physics2D.OverlapPointNonAlloc(
            groundCheck.position,
            results,
            groundLayerMask) == 0;
    }

    private void UpdateCanFlip(Vector2 dir)
	{
        canFlip = !SameDirection(dir);
    }
    
    private bool SameDirection(Vector2 dir)
    {
        var localEulerAngles = transform.localEulerAngles;
        return !(dir.x < 0 && localEulerAngles.y < 180f ||
                 dir.x > 0 && localEulerAngles.y >= 180f);
    }

    private void Flip()
    {
        Vector3 localRotation = transform.localEulerAngles;
        localRotation.y += 180f;
        transform.localEulerAngles = localRotation;
        lifebarCanvas.transform.forward = mainCamera.transform.forward;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        animator.SetTrigger(AnimHurt);
        life -= damage;
        if (life < 0f) life = 0f;
        UpdateLifebar();
        if (life == 0f) Die();
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
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void UpdateMovement(Vector2 newMov)
    {
        newMov.Normalize();
        /*so he doesn't jump*/
        newMov.y = 0f; 
        movement = newMov;
    }
}
