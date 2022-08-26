using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private bool canFlip;

        private void FixedUpdate()
        {
            if (HuyManager.PlayerIsDeath() && enemyHealth.EnemyDeath())
            {
                enemyHealth.EnemyRespawn();
            }
            else if (HuyManager.PlayerIsDeath() && !enemyHealth.EnemyDeath())
            {
                enemyHealth.ResetHeathDefault();
            }

            if (!HuyManager.PlayerIsDeath() && !enemyHealth.EnemyDeath())
            {
                HuyManager.SetTimeAttack(ref currentTime);
                if (isRangeAttack)
                {
                    if (canFlip)
                    {
                        Flip();
                    }

                    if (currentTime <= 0f)
                    {
                        animator.SetTrigger("isAttack");
                        StartCoroutine(DurationAttack(0.5f));
                        currentTime = maxTimeAttack;
                    }
                }
            }
        }

        private IEnumerator DurationAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            AttackBullet();
        }

    }
}