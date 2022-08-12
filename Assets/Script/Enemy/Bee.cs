using System.Collections;
using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {

        [SerializeField] private Vector2 posAttack = Vector2.zero;
        [SerializeField] private Vector2 rangerAttack = Vector2.zero;
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
                    //if (!CheckAttack(transform.position + (Vector3) posAttack, rangerAttack)) return;
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
            enemyManager.AttackBulletDirection(offsetAttack.position);
        }
    }
}