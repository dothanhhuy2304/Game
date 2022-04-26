using UnityEngine;

namespace Game.Core
{
    public class AnimationStates
    {
        public readonly int playerRun = Animator.StringToHash("m_Run");

        public readonly int playerIsJump = Animator.StringToHash("is_Jump");

        public readonly int playerIsDBJump = Animator.StringToHash("is_DB_Jump");

        public readonly int playerIsDash = Animator.StringToHash("is_Dash");

        public readonly int playerIsHurt = Animator.StringToHash("is_Hurt");

        public readonly int playerIsDeath = Animator.StringToHash("is_Death");

        //Pet
        public readonly int petIsRun = Animator.StringToHash("isRun");

        public readonly int petIsDeath = Animator.StringToHash("isDeath");

        //carnivorousPlant
        public readonly int carnivorousPlantIsAttack = Animator.StringToHash("isAttack");

        //SNinja
        public readonly int sNinjaIsRun = Animator.StringToHash("isRun");

        public readonly int sNinjaIsAttack1 = Animator.StringToHash("isAttack1");
        
        //Bee
        public readonly int beeIsAttack = Animator.StringToHash("isAttack");
        
        //Chicken
        public readonly int chickenIsAttack = Animator.StringToHash("is_Run");

        //fireTrap
        public readonly int fireTrapHit = Animator.StringToHash("hit");
        public readonly int fireTrapON = Animator.StringToHash("on");

    }
}
