using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Witch : Enemy
{
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private AudioClip attackAudioClip;

    private readonly float shootVelocity = 5f;

    protected override void Init()
    {
        life = maxHealth = 3f;
        sensingRange = 15f;
    }

    protected override void Attack()
    {
        GameObject throwable = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);
        throwable.GetComponent<Rigidbody2D>().velocity = throwPoint.right * shootVelocity;
        audioSource.PlayOneShot(attackAudioClip);
    }
}

 