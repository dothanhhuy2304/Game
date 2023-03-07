using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private bool canFlip;

        private void FixedUpdate()
        {
            if (HuyManager.PlayerIsDeath() && enemySetting.enemyHeal.EnemyDeath())
            {
                enemySetting.enemyHeal.EnemyRespawn();
            }
            else if (HuyManager.PlayerIsDeath() && !enemySetting.enemyHeal.EnemyDeath())
            {
                enemySetting.enemyHeal.ResetHeathDefault();
            }

            if (!HuyManager.PlayerIsDeath() && !enemySetting.enemyHeal.EnemyDeath())
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