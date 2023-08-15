using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;
using Script.GamePlay;

namespace Script.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        [SerializeField] private bool canFlip;
        private static readonly int IsAttack = Animator.StringToHash("isAttack");
        private bool canAttack;

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
        }

        private void FixedUpdate()
        {
            HuyManager.Instance.SetUpTime(ref currentTime);
            photonView.RPC(nameof(RpcFindPlayerClosets), RpcTarget.All);
            if (canAttack)
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
                            photonView.RPC(nameof(RpcAnimationAttack), RpcTarget.All);
                            DOTween.Sequence()
                                .AppendInterval(0.5f)
                                .AppendCallback(() => { photonView.RPC(nameof(RpcPlanetBulletAttack), RpcTarget.All); })
                                .Play();
                            currentTime = maxTimeAttack;
                        }
                    }
                }
            }
        }


        [PunRPC]
        private void RpcFindPlayerClosets()
        {
            currentCharacterPos = FindClosetPlayerWithForwardPhysic();
            if (currentCharacterPos != null)
            {
                canAttack = true;
            }
            else
            {
                canAttack = false;
            }
        }
        
        [PunRPC]
        private void RpcPlanetBulletAttack()
        {
            AttackBullet();
        }

        [PunRPC]
        private void RpcAnimationAttack()
        {
            animator.SetTrigger(IsAttack);
        }
    }
}