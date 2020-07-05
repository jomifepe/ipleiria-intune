using UnityEngine;

namespace Model
{
    [System.Serializable]
    public class Song
    {
        public AudioClip clip;
        public Buff buff;
        
        public Song(AudioClip clip, Buff buff)
        {
            this.clip = clip;
            this.buff = buff;
        }
    }
}