using UnityEngine;

namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private Vector2 posAttack = Vector2.zero;
        [SerializeField] private Vector2 rangerAttack = Vector2.zero;
        [SerializeField] private bool canFlip;

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
            if (!CheckAttack(transform.position + (Vector3) posAttack, rangerAttack)) return;
            if (canFlip)
            {
                Flip();
            }

            if (currentTime != 0f) return;
            animator.SetTrigger(animationState.carnivorousPlantIsAttack);
            currentTime = maxTimeAttack;
            Attack();
        }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawCube(transform.position + (Vector3) posAttack, rangerAttack);
        // }
    }
}