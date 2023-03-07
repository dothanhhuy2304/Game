using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {
        private static readonly int IsAttack = Animator.StringToHash("isAttack");

        private void Update()
        {
            if (HuyManager.PlayerIsDeath() && enemySetting.enemyHeal.EnemyDeath())
            {
                enemySetting.enemyHeal.EnemyRespawn();
            }

            if (HuyManager.PlayerIsDeath())
            {
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    enemySetting.enemyHeal.ResetHeathDefault();
                }
            }

            if (!HuyManager.PlayerIsDeath())
            {
                HuyManager.SetTimeAttack(ref currentTime);
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    if (isRangeAttack)
                    {
                        Flip();
                        if (currentTime <= 0)
                        {
                            if (!HuyManager.PlayerIsDeath() || !enemySetting.enemyHeal.EnemyDeath())
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            EvaluateCheckRangeAttack(other, true);
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGrounds = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            EvaluateCheckRangeAttack(other, false);
            if (other.CompareTag("ground"))
            {
                isHitGrounds = false;
            }
        }
        

    }
}