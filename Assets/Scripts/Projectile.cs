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
            PlaySound(onHitSound);
            HideObject();
        }
        else if (other.CompareTag("Player") && type == Type.Spell)
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            PlaySound(onHitSound);
            HideObject();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Platform") || other.CompareTag("FallPlatform"))
        {
            PlaySound(onHitWallSound);
            HideObject();
        }
    }

    private void HideObject()
    {
        spriteRenderer.enabled = false;
    }
    
    private void PlaySound(AudioClip sound)
    {
        if (sound == null) return;
        StartCoroutine(PlaySoundAndDestroy(sound));
    }

    private IEnumerator PlaySoundAndDestroy(AudioClip sound)
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
