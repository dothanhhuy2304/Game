using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioSource landAudioSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;

    public void PlayerJump()
    {
        jumpAudioSource.PlayOneShot(jumpClip);
    }

    public void PlayerLand()
    {
        landAudioSource.PlayOneShot(landClip);
    }

}
