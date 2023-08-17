using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Trunk : EnemyController , IPunObservable
    {
        [SerializeField] private float rangeAttack = 10f;

        private void Update()
        {
            RpcTargetPosition();
            if (enemySetting.canAttack)
            {
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    HuyManager.Instance.SetUpTime(ref CurrentTime);
                    if ((currentCharacterPos.transform.position - transform.position).magnitude < rangeAttack)
                    {
                        Flip();
                        if (CurrentTime <= 0)
                        {
                            Shot();
                        }
                    }
                }
            }
        }

        private void RpcTargetPosition()
        {
            currentCharacterPos = FindClosestPlayer();
            enemySetting.canAttack = currentCharacterPos;
        }

        private void Shot()
        {
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(AttackBullet)
                .Play();
            animator.SetTrigger("isAttack");
            CurrentTime = maxTimeAttack;
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