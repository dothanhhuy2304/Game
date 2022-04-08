using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource playerAudioSource;
    public AudioSource bothAudioSource;
    [SerializeField] private AudioClip jumpClip;

    public void PlayerJump()
    {
        playerAudioSource.PlayOneShot(jumpClip);
    }

    public void PlayerLand(AudioClip clip)
    {
        playerAudioSource.PlayOneShot(clip);
    }

    public void Play(AudioClip clip)
    {
        bothAudioSource.PlayOneShot(clip,1f);
    }

}
