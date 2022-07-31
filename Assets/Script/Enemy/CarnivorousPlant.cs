using Game.Core;
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
            if (HuyManager.PlayerIsDeath() && enemyHealth.EnemyDeath())
            {
                enemyHealth.EnemyRespawn();
            }

            if (HuyManager.PlayerIsDeath())
            {
                if (!enemyHealth.EnemyDeath())
                {
                    enemyHealth.ResetHeathDefault();
                }
            }

            if (HuyManager.PlayerIsDeath()) return;
            BaseObject.SetTimeAttack(ref currentTime);
            if (enemyHealth.EnemyDeath()) return;
            if (!CheckAttack(transform.position + (Vector3) posAttack, rangerAttack)) return;
            if (canFlip)
            {
                Flip();
            }

            if (currentTime != 0f) return;
            animator.SetTrigger("isAttack");
            currentTime = maxTimeAttack;
            Attack();
        }

    }
}