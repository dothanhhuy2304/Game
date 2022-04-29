using Game.GamePlay;
using UnityEngine;

public class SoundRunAnim : StateMachineBehaviour
{
    public float t = 0.5f;
    public float modulus = 0f;
    private float lastT = -1f;
    [SerializeField] private string clipName;


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var nt = stateInfo.normalizedTime;
        if (modulus > 0f) nt %= modulus;
        if (nt >= t && lastT < t)
            PlayerAudio.Instance.Play(clipName);
        lastT = nt;
    }
}