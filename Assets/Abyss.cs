using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abyss : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D otherCollider = other.collider;
        if (otherCollider.CompareTag("Player"))
        {
            otherCollider.GetComponent<PlayerController>().Kill();
        }
    }
}
