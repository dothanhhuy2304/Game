using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {

        private void Update()
        {
            CheckUpdate();
            if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            Flip();
            if (currentTime != 0) return;
            animator.SetTrigger(animationState.beeIsAttack);
            Attack();
            currentTime = maxTimeAttack;
        }
    }
}