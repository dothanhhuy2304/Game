using System.Collections.Generic;
using Game.Core;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class Trunk : EnemyController
    {
        [SerializeField] private List<ProjectileArc> projectileArcs;
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
                HuyManager.SetTimeAttack(ref currentTime);
                if (!enemyHealth.EnemyDeath())
                {
                    //if (CheckAttack(transform.position + (Vector3) posAttack, rangerAttack))
                    if (isRangeAttack)
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

        private void AttackBulletArc()
        {
            projectileArcs[FindBullet(projectileArcs)].transform.rotation = Quaternion.identity;
            projectileArcs[FindBullet(projectileArcs)].transform.position = offsetAttack.position;
            projectileArcs[FindBullet(projectileArcs)].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
    }
}