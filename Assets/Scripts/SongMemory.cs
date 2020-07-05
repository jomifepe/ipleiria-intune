using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class SongMemory : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    [SerializeField] private Buff buff;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.PreviewSong(song);
            GameManager.Instance.AddSong(new Song(song, buff));
            Destroy(gameObject);
        }
    }
}
