using System;
using System.Collections;
using Model;
using UnityEngine;
using UnityEngine.Audio;
using Object = System.Object;

public class Projectile : MonoBehaviour
{
    private enum Type { Tomahawk, Spell }
    [SerializeField] private Type type;
    [SerializeField] private float damage = 1f;
    [SerializeField] private AudioClip onHitSound;
    [SerializeField] private AudioClip onHitWallSound;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    
    public Buff buff = Buff.None;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && type == Type.Tomahawk)
        {
            if(!other.GetComponent<Enemy.Enemy>().TakeDamage(damage, buff)) return;
            PlaySoundAndDestroy(onHitSound);
        }
        else if (other.CompareTag("Player") && type == Type.Spell)
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            PlaySoundAndDestroy(onHitSound);
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Platform") || other.CompareTag("FallPlatform"))
        {
            PlaySoundAndDestroy(onHitWallSound);
        }
    }

    private void HideObject()
    {
        spriteRenderer.enabled = false;
    }
    
    private void PlaySoundAndDestroy(AudioClip sound)
    {
        HideObject();
        if (sound == null) Destroy(gameObject);
        StartCoroutine(PlaySoundAndDestroyAsync(sound));
    }

    private IEnumerator PlaySoundAndDestroyAsync(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(sound.length);
        Destroy(gameObject);
    }

    private void Dismiss()
    {
        Destroy(gameObject);
    }
}
