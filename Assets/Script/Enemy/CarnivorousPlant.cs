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

            if (HuyManager.PlayerIsDeath() && !enemyHealth.EnemyDeath())
            {
                enemyHealth.ResetHeathDefault();
            }

            if (!HuyManager.PlayerIsDeath())
            {
                HuyManager.SetTimeAttack(ref currentTime);
                if (!enemyHealth.EnemyDeath())
                {
                    if (CheckAttack(transform.position + (Vector3) posAttack, rangerAttack))
                    {
                        if (canFlip)
                        {
                            Flip();
                        }

                        if (currentTime != 0f) return;
                        animator.SetTrigger("isAttack");
                        if (!HuyManager.PlayerIsDeath() || !enemyHealth.EnemyDeath())
                        {
                            StartCoroutine(DurationAttack(0.5f));
                        }

                        currentTime = maxTimeAttack;
                    }
                }
            }
        }

        private System.Collections.IEnumerator DurationAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            AttackBullet();
        }
    }
}