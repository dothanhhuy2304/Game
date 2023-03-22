using DG.Tweening;
using UnityEngine;

namespace Game.Enemy
{
    public class Trunk : EnemyController
    {
        [SerializeField] private float rangeAttack = 10f;
        [SerializeField] private LayerMask mask;

        private void Update()
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
                if ((playerCharacter.transform.position - transform.position).magnitude < rangeAttack)
                {
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, playerCharacter.transform.position, mask);
                    if (hit)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            Flip();
                            if (currentTime <= 0)
                            {
                                DurationAttack(0.5f);
                                animator.SetTrigger("isAttack");
                                currentTime = maxTimeAttack;
                            }
                        }
                    }
                }
            }
        }

        private void DurationAttack(float duration)
        {
            DOTween.Sequence()
                .AppendInterval(duration)
                .AppendCallback(AttackBullet)
                .Play();
        }
    }
}