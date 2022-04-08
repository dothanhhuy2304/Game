using UnityEngine;

public class SoundRunAnim : StateMachineBehaviour
{
    public float t = 0.5f;
    public float modulus = 0f;
    public AudioClip clip;
    private float lastT = -1f;
    private PlayerAudio playerAudio;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var nt = stateInfo.normalizedTime;
        if (modulus > 0f) nt %= modulus;
        if (nt >= t && lastT < t)
            //AudioSource.PlayClipAtPoint(clip, animator.transform.position, 1f);
            playerAudio.PlayerLand(clip);
        lastT = nt;
    }
}