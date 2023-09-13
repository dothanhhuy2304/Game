using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Trunk : EnemyController , IPunObservable
    {
        [SerializeField] private float rangeAttack = 10f;

        private void Awake()
        {
            if (pv == null)
            {
                pv = GetComponent<PhotonView>();
            }
        }

        private void Update()
        {
            FindPlayerPosition();
            if (enemySetting.canAttack)
            {
                if (!enemySetting.enemyHeal.EnemyDeath())
                {
                    HuyManager.Instance.SetUpTime(ref CurrentTime);
                    if ((currentCharacterPos.transform.position - transform.position).magnitude < rangeAttack)
                    {
                        if (pv.IsMine)
                        {
                            Flip();
                        }
                        
                        pv.RPC(nameof(RpcShot), RpcTarget.AllBuffered);
                    }
                }
            }
        }

        private void FindPlayerPosition()
        {
            currentCharacterPos = FindClosestPlayer();
            enemySetting.canAttack = currentCharacterPos;
        }

        [PunRPC]
        private void RpcShot()
        {
            if (CurrentTime <= 0)
            {
                Shot();
            }
        }

        private void Shot()
        {
            animator.SetTrigger("isAttack");
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(() => { AttackBulletByPlayer(currentCharacterPos); })
                .Play();
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