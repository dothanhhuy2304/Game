using UnityEngine;

namespace Game.Core
{
    public class AnimationStates
    {
        public readonly int playerRun = Animator.StringToHash("m_Run");

        public readonly int playerIsJump = Animator.StringToHash("is_Jump");

        public readonly int playerIsHurt = Animator.StringToHash("is_Hurt");

        //Pet
        public readonly int petIsRun = Animator.StringToHash("isRun");

        public readonly int petIsDeath = Animator.StringToHash("isDeath");

        //carnivorousPlant
        public readonly int carnivorousPlantIsAttack = Animator.StringToHash("isAttack");

        //SNinja
        public readonly int sNinjaIsRun = Animator.StringToHash("isRun");

        public readonly int sNinjaIsAttack1 = Animator.StringToHash("isAttack1");

        //fireTrap
        public readonly int fireTrapHit = Animator.StringToHash("hit");
        public readonly int fireTrapON = Animator.StringToHash("on");

    }
}
