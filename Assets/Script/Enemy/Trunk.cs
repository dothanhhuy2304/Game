using Game.Enemy;

public class Trunk : EnemyController
{

    [UnityEngine.SerializeField] private CheckEnemyAttack checkEnemyAttack;

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
        SetTimeAttack(ref currentTime);
        if (enemyHealth.EnemyDeath()) return;
        if (!isVisible) return;
        if (!checkEnemyAttack.canAttack) return;
        Flip();
        if (currentTime != 0) return;
        animator.SetTrigger(animationState.trunkIsAttack);
        Attack();
        currentTime = maxTimeAttack;
    }
}