using System;
using Script.GamePlay;
using UnityEngine;

namespace Script.Player
{
    public class AudioManager : FastSingleton<AudioManager>
    {
        public AudioSource audioMusic;
        public Sound[] sounds;

        protected override void Awake()
        {
            base.Awake();
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