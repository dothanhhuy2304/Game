using DG.Tweening;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Bee : EnemyController
    {
        private static readonly int IsAttack = Animator.StringToHash("isAttack");
        private bool _canAttack;

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            RpcFindClosetPlayer();
            if (!enemySetting.enemyHeal.EnemyDeath())
            {
                HuyManager.Instance.SetUpTime(ref currentTime);
                if (_canAttack)
                {
                    if ((currentCharacterPos.transform.position - transform.position).magnitude < enemySetting.rangeAttack)
                    {
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
        }

        private void RpcFindClosetPlayer()
        {
            currentCharacterPos = FindClosestPlayer();
            if (currentCharacterPos != null)
            {
                _canAttack = true;
            }
            else
            {
                _canAttack = false;
            }
        }

        private void BulletAttack()
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(AttackBulletDirection)
                .Play();
        }
    }
}