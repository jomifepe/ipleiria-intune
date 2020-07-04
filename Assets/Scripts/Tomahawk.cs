using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomahawk : MonoBehaviour
{
    [SerializeField]
    private float damage = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(damage);
            Dismiss();
        }
        else
        {
            if (other.CompareTag("Wall"))
            {
                Dismiss();
            }
        }
    }

    private void Dismiss()
    {
        Destroy(gameObject);
    }
}
