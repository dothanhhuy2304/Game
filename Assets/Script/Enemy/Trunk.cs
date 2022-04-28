using Game.Enemy;
using UnityEngine;

public class Trunk : EnemyController
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

        if (playerHealth.PlayerIsDeath()) return;
        if (enemyHealth.EnemyDeath()) return;
        if (!isVisible) return;
        TimeAttack();
        if (!(Vector2.Distance(transform.position, player.position) < rangeAttack)) return;
        Flip();
        if (currentTime != 0) return;
        animator.SetTrigger(animationState.trunkIsAttack);
        Attack();
        currentTime = maxTimeAttack;
    }
}
