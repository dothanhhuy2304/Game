using UnityEngine;

namespace Game.Core
{
    public class AnimationState
    {
        public readonly int playerRun = Animator.StringToHash("m_Run");

        public readonly int playerIsJump = Animator.StringToHash("is_Jump");

        //Pet
        public readonly int petIsRun = Animator.StringToHash("isRun");

        public readonly int petIsDeath = Animator.StringToHash("isDeath");

        //
        public readonly int carnivorousPlantIsAttack = Animator.StringToHash("isAttack");

        //
        public readonly int sNinjaIsRun = Animator.StringToHash("isRun");
        public readonly int sNinjaIsAttack1 = Animator.StringToHash("isAttack1");

    }
}
