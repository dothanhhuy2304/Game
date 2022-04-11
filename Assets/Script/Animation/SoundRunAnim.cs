using UnityEngine;

public class SoundRunAnim : StateMachineBehaviour
{
    public float t = 0.5f;
    public float modulus = 0f;
    private float lastT = -1f;
    private PlayerAudio playerAudio;
    [SerializeField] private string clipName;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var nt = stateInfo.normalizedTime;
        if (modulus > 0f) nt %= modulus;
        if (nt >= t && lastT < t)
            playerAudio.Plays_10(clipName);
        lastT = nt;
    }
}