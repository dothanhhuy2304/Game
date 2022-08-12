using System.Collections;
using System.Collections.Generic;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {
        [SerializeField] private List<FireProjectile> projectiles;
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
            AttackBulletDirection();
        }
        
        private void AttackBulletDirection()
        {
            Vector2 directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
    }
}