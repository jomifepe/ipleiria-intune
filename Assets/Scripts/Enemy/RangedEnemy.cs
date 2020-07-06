using UnityEngine;

namespace Enemy
{
    public abstract class RangedEnemy : EnemyNonAI
    {
        [SerializeField] private GameObject throwablePrefab;
        [SerializeField] private float shootVelocity;

        protected override void Attack()
        {
            GameObject throwable = Instantiate(throwablePrefab, attackPoint.position, attackPoint.rotation);
            throwable.GetComponent<Rigidbody2D>().velocity = attackPoint.right * shootVelocity;
            audioSource.PlayOneShot(attackAudioClip);
        }
    }
}
