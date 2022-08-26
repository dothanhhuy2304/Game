using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {
        private static readonly int IsAttack = Animator.StringToHash("isAttack");

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

            if (!HuyManager.PlayerIsDeath())
            {
                HuyManager.SetTimeAttack(ref currentTime);
                if (!enemyHealth.EnemyDeath())
                {
                    if (isRangeAttack)
                    {
                        Flip();
                        if (currentTime <= 0)
                        {
                            if (!HuyManager.PlayerIsDeath() || !enemyHealth.EnemyDeath())
                            {
                                StartCoroutine(DurationAttack(0.5f));
                            }

                            animator.SetTrigger(IsAttack);
                            currentTime = maxTimeAttack;
                        }
                    }
                }
            }
        }

        private IEnumerator DurationAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            AttackBulletDirection();
        }

    }
}