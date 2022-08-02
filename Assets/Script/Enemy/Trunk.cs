using Game.Core;
using UnityEngine;

namespace Game.Enemy
{
    public class Trunk : EnemyController
    {

        [SerializeField] private Vector2 posAttack = Vector2.zero;
        [SerializeField] private Vector2 rangerAttack = Vector2.zero;

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
                BaseObject.SetTimeAttack(ref currentTime);
                if (!enemyHealth.EnemyDeath())
                {
                    if (CheckAttack(transform.position + (Vector3) posAttack, rangerAttack))
                    {
                        Flip();
                        if (currentTime != 0) return;
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
            AttackBulletArc();
        }

    }
}