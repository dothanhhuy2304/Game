using DG.Tweening;
using UnityEngine;

namespace Game.Enemy
{
    public class Trunk : EnemyController
    {
        [SerializeField] private float rangeAttack = 10f;
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

            if (!HuyManager.PlayerIsDeath())
            {
                HuyManager.SetTimeAttack(ref currentTime);
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    if ((playerCharacter.transform.position - transform.position).magnitude < rangeAttack)
                    {
                        RaycastHit2D hit = Physics2D.Linecast(transform.position, playerCharacter.transform.position, 1 << LayerMask.NameToLayer("ground"));
                        if (hit && hit.collider.CompareTag("ground"))
                        {
                            return;
                        }

                        Flip();
                        if (currentTime <= 0)
                        {
                            if (!HuyManager.PlayerIsDeath() || !enemySetting.enemyHeal.EnemyDeath())
                            {
                                DurationAttack(0.5f);
                            }

                            animator.SetTrigger("isAttack");
                            currentTime = maxTimeAttack;
                        }
                    }
                }
            }
        }

        private void DurationAttack(float duration)
        {
            DOTween.Sequence()
                .AppendInterval(duration)
                .AppendCallback(AttackBulletArc).Play();
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