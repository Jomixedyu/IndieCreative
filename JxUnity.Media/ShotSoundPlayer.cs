using System;
using UnityEngine;

namespace JxUnity.Media
{
    [RequireComponent(typeof(AudioSource))]
    public class ShotSoundPlayer : MonoBehaviour
    {
        private static ShotSoundPlayer instance;
        public static ShotSoundPlayer GetInstance()
        {
            if (instance == null)
            {
                var go = new GameObject($"__m_{nameof(ShotSoundPlayer)}");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<ShotSoundPlayer>();
            }
            return instance;
        }

        private AudioSource audioSource;
        private Func<float> volumeGetter;

        private void Awake()
        {
            this.audioSource = GetComponent<AudioSource>();
            if (this.audioSource == null)
            {
                this.audioSource = this.gameObject.AddComponent<AudioSource>();
            }
        }



        public void SetVolumeGetter(Func<float> getter)
        {
            this.volumeGetter = getter;
        }

        public void Play(AudioClip audioClip, float volume = 1f)
        {
            this.audioSource.PlayOneShot(audioClip, volume);
        }

        public void Play(AudioClip audioClip)
        {
            float volume = this.volumeGetter?.Invoke() ?? 1f;
            this.audioSource.PlayOneShot(audioClip, volume);
        }
    }
}
