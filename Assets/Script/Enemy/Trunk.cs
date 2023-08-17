using DG.Tweening;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Trunk : EnemyController
    {
        [SerializeField] private float rangeAttack = 10f;
        [SerializeField] private LayerMask mask;

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            RpcPlayerPosition();
            if (enemySetting.canAttack)
            {
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    HuyManager.Instance.SetUpTime(ref currentTime);
                    if ((currentCharacterPos.transform.position - transform.position).magnitude < rangeAttack)
                    {
                        Flip();
                        if (currentTime <= 0)
                        {
                            Shot();
                        }
                    }
                }
            }
        }

        private void RpcPlayerPosition()
        {
            currentCharacterPos = FindClosestPlayer();
            if (currentCharacterPos!=null)
            {
                enemySetting.canAttack = true;
            }
            else
            {
                enemySetting.canAttack = false;
            }
        }

        private void Shot()
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(AttackBullet)
                .Play();
            animator.SetTrigger("isAttack");
            currentTime = maxTimeAttack;
        }
    }
}