using UnityEngine;

namespace Enemy
{
    public abstract class MeleeEnemy : Enemy
    {
        [SerializeField] private LayerMask playerLayerMask;
        protected override void Attack()
        {
            if (Physics2D.OverlapCircleNonAlloc(attackPoint.position, attackRange, results, playerLayerMask) == 0) return;
            results[0].GetComponent<PlayerController>().TakeDamage(attackDamage);
            audioSource.PlayOneShot(attackAudioClip);
        }
    }
}
