
namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        private CheckEnemyAttack checkEnemyAttack;

        private void Awake()
        {
            checkEnemyAttack = GetComponentInChildren<CheckEnemyAttack>();
        }

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
            if (playerHealth.PlayerIsDeath()) return;
            if (enemyHealth.EnemyDeath()) return;
            //if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            if (!checkEnemyAttack.canAttack) return;
            Flip();
            if (currentTime != 0f) return;
            animator.SetTrigger(animationState.carnivorousPlantIsAttack);
            currentTime = maxTimeAttack;
            Attack();
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawSphere(transform.position, rangeAttack);
        // }
    }
}