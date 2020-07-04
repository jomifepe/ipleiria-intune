using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongMemory : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            // AudioManager.Instance.
        }
    }
}
