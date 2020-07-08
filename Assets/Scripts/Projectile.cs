using Model;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private enum Type { Tomahawk, Spell }
    [SerializeField] private Type type;
    [SerializeField] private float damage = 1f;
    public Buff buff = Buff.None;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && type == Type.Tomahawk)
        {
            if(!other.GetComponent<Enemy.Enemy>().TakeDamage(damage, buff)) return;
            Dismiss();
        }
        else if (other.CompareTag("Player") && type == Type.Spell)
        {
            //other.GetComponent<PlayerController>().TakeDamage(damage);
            Dismiss();
        }
        else if (other.CompareTag("Wall"))
        {
            Dismiss();
        }
    }
    private void Dismiss()
    {
        Destroy(gameObject);
    }
}
