using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {
        private void Update()
        {
            if (playerHealth.PlayerIsDeath() && enemyHealth.EnemyDeath())
            {
                enemyHealth.EnemyRespawn();
            }

            if (playerHealth.PlayerIsDeath())
            {
                if (!enemyHealth.EnemyDeath())
                {
                    enemyHealth.ResetHeathDefault();
                }
            }

            if (!isVisible) return;
            if (enemyHealth.EnemyDeath()) return;
            TimeAttack();
            if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            Flip();
            if (currentTime != 0) return;
            animator.SetTrigger(animationState.beeIsAttack);
            Attack();
            currentTime = maxTimeAttack;
        }
    }
}