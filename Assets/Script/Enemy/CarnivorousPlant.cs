
namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
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
            //if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            if (!checkEnemyAttack.canAttack) return;
            Flip();
            if (currentTime != 0f) return;
            animator.SetTrigger(animationState.carnivorousPlantIsAttack);
            currentTime = maxTimeAttack;
            Attack();
        }
    }
}