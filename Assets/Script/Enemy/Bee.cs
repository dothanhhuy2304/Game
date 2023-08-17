using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Bee : EnemyController , IPunObservable
    {
        private readonly int _isAttack = Animator.StringToHash("isAttack");
        private bool _canAttack;

        private void Update()
        {
            RpcTargetPosition();
            if (!enemySetting.enemyHeal.EnemyDeath())
            {
                HuyManager.Instance.SetUpTime(ref CurrentTime);
                if (_canAttack)
                {
                    if ((currentCharacterPos.transform.position - transform.position).magnitude < enemySetting.rangeAttack)
                    {
                        Flip();
                        if (CurrentTime <= 0)
                        {
                            BulletAttack();
                            animator.SetTrigger(_isAttack);
                            CurrentTime = maxTimeAttack;
                        }
                    }
                }
            }
        }

        private void RpcTargetPosition()
        {
            currentCharacterPos = FindClosestPlayer();
            _canAttack = currentCharacterPos;
        }

        private void BulletAttack()
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(AttackBulletDirection)
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