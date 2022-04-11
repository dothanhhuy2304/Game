using System;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource10;
    [SerializeField] private AudioSource audioSource13;
    [SerializeField] private AudioSource audioSource20;
    [SerializeField] private Sound[] sounds;

    public void Plays_10(string clip)
    {
        var s = Array.Find(sounds, sound => sound.name == clip);
        audioSource10.PlayOneShot(s.audioClip);
    }

    public void Plays_13(string clip)
    {
        var s = Array.Find(sounds, sound => sound.name == clip);
        audioSource13.PlayOneShot(s.audioClip);
    }

    public void Plays_20(string clip)
    {
        var s = Array.Find(sounds, sound => sound.name == clip);
        audioSource20.PlayOneShot(s.audioClip);
    }
}