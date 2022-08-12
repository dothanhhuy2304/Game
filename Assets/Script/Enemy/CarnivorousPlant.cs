using System.Collections;
using System.Collections.Generic;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] private Vector2 posAttack = Vector2.zero;
        [SerializeField] private Vector2 rangerAttack = Vector2.zero;
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
                //if (CheckAttack(transform.position + (Vector3) posAttack, rangerAttack))
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

        private void AttackBullet()
        {
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].transform.rotation = transform.rotation;
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
    }
}