using System;
using UnityEngine;

namespace Game.GamePlay
{

    public class PlayerAudio : MonoBehaviour
    {
        private static PlayerAudio _instance;

        [Space] [Header("Sound Music")] [SerializeField]
        private AudioSource audioMusic;

        [Space] [Header("Clip")] [SerializeField]
        public Sound[] sounds;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            foreach (var s in sounds)
            {
                s.audioFX = gameObject.AddComponent<AudioSource>();
                s.audioFX.clip = s.audioClip;
                s.audioFX.outputAudioMixerGroup = s.audioMixerGroup;
                s.audioFX.playOnAwake = false;
            }
        }

        public void Plays_Music(string clip)
        {
            var s = Array.Find(sounds, sound => sound.name == clip);
            audioMusic.clip = s.audioClip;
            audioMusic.Play();
        }

        public void Play(string clip)
        {
            var s = Array.Find(sounds, sound => sound.name == clip);
            s.audioFX.PlayOneShot(s.audioClip);
        }
    }
}