using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Animation
{
    public class SoundRunAnim : StateMachineBehaviour
    {
        public float t = 0.5f;
        public float modulus = 0f;
        private float lastT = -1f;
        [SerializeField] private string clipName;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!HuyManager.PlayerIsDeath())
            {
                float nt = stateInfo.normalizedTime;
                if (modulus > 0f) nt %= modulus;
                if (nt >= t && lastT < t)
                    AudioManager.instance.Play(clipName);
                lastT = nt;
            }
        }
    }
}