using UnityEngine;

namespace Game.Enemy
{
    public class Trunk : EnemyController
    {

        [SerializeField] private Vector2 posAttack = Vector2.zero;
        [SerializeField] private Vector2 rangerAttack = Vector2.zero;

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
            if (!CheckAttack(transform.position + (Vector3) posAttack, rangerAttack)) return;
            Flip();
            if (currentTime != 0) return;
            animator.SetTrigger(animationState.trunkIsAttack);
            Attack();
            currentTime = maxTimeAttack;
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawCube(transform.position + (Vector3) posAttack, rangerAttack);
        // }
    }
}