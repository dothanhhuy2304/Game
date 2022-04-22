using UnityEngine;

namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        private void Update()
        {
            if (playerHealth.PlayerIsDeath())
            {
                enemyHealth.ResetHeathDefault();
                if (enemyHealth.EnemyDeath())
                {
                    enemyHealth.EnemyRespawn();
                }
            }

            if (!isVisible) return;
            TimeAttack();
            if (playerHealth.PlayerIsDeath()) return;
            if (enemyHealth.EnemyDeath()) return;
            if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            Flip();
            if (currentTime != 0f) return;
            animator.SetTrigger(animationState.carnivorousPlantIsAttack);
            currentTime = maxTimeAttack;
            Attack();
        }
    }
}