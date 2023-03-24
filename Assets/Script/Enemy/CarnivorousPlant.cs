using DG.Tweening;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private bool canFlip;
        [SerializeField] private LayerMask mask;

        private void FixedUpdate()
        {
            if (HuyManager.PlayerIsDeath())
            {
                if (enemySetting.enemyHeal.EnemyDeath())
                {
                    enemySetting.enemyHeal.ResetHeathDefault();
                    enemySetting.enemyHeal.ReSpawn(2);
                }
                else
                {
                    DOTween.Sequence()
                        .AppendInterval(2f)
                        .AppendCallback(enemySetting.enemyHeal.ResetHeathDefault)
                        .Play();
                }
            }

            if (!HuyManager.PlayerIsDeath() && !enemySetting.enemyHeal.EnemyDeath())
            {
                HuyManager.SetTimeAttack(ref currentTime);
                if ((playerCharacter.transform.position - transform.position).magnitude < enemySetting.rangeAttack)
                {
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, playerCharacter.transform.position, mask);
                    if (hit)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            if (canFlip)
                            {
                                Flip();
                            }

                            if (currentTime <= 0f)
                            {
                                BulletAttack();
                                animator.SetTrigger("isAttack");
                                currentTime = maxTimeAttack;
                            }
                        }
                    }
                }
            }
        }

        private void BulletAttack()
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(AttackBullet)
                .Play();
        }
    }
}