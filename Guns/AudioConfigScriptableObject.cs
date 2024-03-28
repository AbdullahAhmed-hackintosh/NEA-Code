using UnityEngine;

namespace FistOfTheFree.Guns
{
    // Scriptable object for Audio Config
    [CreateAssetMenu(fileName = "Audio Config", menuName = "Guns/Audio Config", order = 5)]
    public class AudioConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        [Range(0, 1f)]
        public float Volume = 1f; // Volume
        public AudioClip[] FireClips; // audio source file (.mp3) to be played when the gun is firing
        public AudioClip EmptyClip; // audio to be played for an empty clip
        public AudioClip ReloadClip; // audio to be played when reloading
        public AudioClip LastBulletClip; // audio to be played when the last bullet in the gun.

        // if gun has bullets, FireClips audio will play else EmptyClip will play
        public void PlayShootingClip(AudioSource AudioSource, bool IsLastBullet = false)
        {
            if (IsLastBullet && LastBulletClip != null) 
            {
                AudioSource.PlayOneShot(LastBulletClip, Volume);
            }
            else
            {
                AudioSource.PlayOneShot(FireClips[Random.Range(0, FireClips.Length)], Volume);
            }
        }


        public void PlayOutOfAmmoClip(AudioSource AudioSource)
        {
            if (EmptyClip != null)
            {
                AudioSource.PlayOneShot(EmptyClip, Volume);
            }
        }

        // Plays reload audio
        public void PlayReloadClip(AudioSource AudioSource)
        {
            if (ReloadClip != null)
            {
                AudioSource.PlayOneShot(ReloadClip, Volume);
            }
        }

        // clones this scriptable object (modifier)
        public object Clone()
        {
            AudioConfigScriptableObject config = CreateInstance<AudioConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}