using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private bool canFlip;
        private bool _canAttack;

        protected override void Start()
        {
            base.Start();
            HuyManager.Instance.eventResetWhenPlayerDeath += WaitToReset;
        }

        private void WaitToReset()
        {

            if (HuyManager.Instance.PlayerIsDeath())
            {
                if (enemySetting.enemyHeal.EnemyDeath())
                {
                    RpcReSpawnCarEnemy();
                }
                else
                {
                    DOTween.Sequence()
                        .AppendInterval(2f)
                        .AppendCallback(RpcResetHeath);
                }
            }
        }

        private void RpcReSpawnCarEnemy()
        {
            enemySetting.enemyHeal.ResetHeathDefault();
            enemySetting.enemyHeal.ReSpawn(2);
        }

        private void RpcResetHeath()
        {
            enemySetting.enemyHeal.ResetHeathDefault();
        }

        private void FixedUpdate()
        {
            RpcFindPlayerClosets();
            HuyManager.Instance.SetUpTime(ref currentTime);

            if (_canAttack)
            {
                if (!HuyManager.Instance.PlayerIsDeath() && !enemySetting.enemyHeal.EnemyDeath())
                {
                    if ((currentCharacterPos.position - transform.position).magnitude < enemySetting.rangeAttack)
                    {
                        if (canFlip)
                        {
                            Flip();
                        }

                        if (currentTime <= 0f)
                        {
                            RpcCarnivorousAttack(0.5f);
                            currentTime = maxTimeAttack;
                        }
                    }
                }
            }
        }

        private void RpcFindPlayerClosets()
        {
            currentCharacterPos = FindClosetPlayerWithForwardPhysic();
            if (currentCharacterPos)
            {
                _canAttack = true;
            }
            else
            {
                _canAttack = false;
            }
        }

        private void RpcCarnivorousAttack(float duration)
        {
            animator.SetTrigger("isAttack");
            DOTween.Sequence()
                .AppendInterval(duration)
                .AppendCallback(AttackBullet)
                .Play();
        }
    }
}