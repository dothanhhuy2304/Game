using UnityEngine;
using UnityEngine.Audio;

namespace Script.GamePlay
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip audioClip;
        [HideInInspector] public AudioSource audioFX;
        public AudioMixerGroup audioMixerGroup;
    }
}
