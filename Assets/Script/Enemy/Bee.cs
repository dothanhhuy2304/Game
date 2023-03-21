using DG.Tweening;
using UnityEngine;

namespace Game.Enemy
{
    public class Bee : EnemyController
    {
        private static readonly int IsAttack = Animator.StringToHash("isAttack");

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
                if ((playerCharacter.transform.position - transform.position).magnitude < enemySetting.rangeAttack)
                {
                    RaycastHit2D hit = Physics2D.Linecast(transform.position, playerCharacter.transform.position, 1 << LayerMask.NameToLayer("ground"));
                    if (hit)
                    {
                        if (hit.collider.CompareTag("ground"))
                        {
                            return;
                        }
                    }

                    Flip();
                    if (currentTime <= 0)
                    {
                        BulletAttack();
                        animator.SetTrigger(IsAttack);
                        currentTime = maxTimeAttack;
                    }
                }
            }
        }

        private void BulletAttack()
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(AttackBulletDirection);
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     EvaluateCheckRangeAttack(other, true);
        // }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGrounds = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            //EvaluateCheckRangeAttack(other, false);
            if (other.CompareTag("ground"))
            {
                isHitGrounds = false;
            }
        }
        

    }
}