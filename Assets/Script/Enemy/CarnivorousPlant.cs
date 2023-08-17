using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class CarnivorousPlant : EnemyController , IPunObservable
    {
        [SerializeField] private bool canFlip;
        private bool _canAttack;
        

        private void FixedUpdate()
        {
            RpcTargetPosition();
            HuyManager.Instance.SetUpTime(ref CurrentTime);

            if (_canAttack)
            {
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    if ((currentCharacterPos.position - transform.position).magnitude < enemySetting.rangeAttack)
                    {
                        if (canFlip)
                        {
                            Flip();
                        }

                        if (CurrentTime <= 0f)
                        {
                            RpcCarnivorousAttack(0.5f);
                            CurrentTime = maxTimeAttack;
                        }
                    }
                }
            }
        }

        private void RpcTargetPosition()
        {
            currentCharacterPos = FindClosetPlayerWithForwardPhysic();
            _canAttack = currentCharacterPos;
        }

        private void RpcCarnivorousAttack(float duration)
        {
            animator.SetTrigger("isAttack");
            DOTween.Sequence()
                .AppendInterval(duration)
                .AppendCallback(AttackBullet)
                .Play();
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((Vector3) body.velocity);
                stream.SendNext((float) body.rotation);
                stream.SendNext((Vector3) transform.position);
                stream.SendNext((Quaternion) transform.rotation);
            }
            else
            {
                body.velocity = (Vector3) stream.ReceiveNext();
                body.rotation = (float) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
            }
        }
    }
}