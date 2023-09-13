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

        private void Awake()
        {
            if (pv == null)
            {
                pv = GetComponent<PhotonView>();
            }
        }

        private void FixedUpdate()
        {
            if (enemySetting.enemyHeal.EnemyDeath())
            {
                return;
            }

            FindPlayerPosition();
            if (_canAttack)
            {
                HuyManager.Instance.SetUpTime(ref CurrentTime);
                if ((currentCharacterPos.position - transform.position).magnitude < enemySetting.rangeAttack)
                {
                    if (pv.IsMine && canFlip)
                    {
                        Flip();
                    }

                    pv.RPC(nameof(RpcShot), RpcTarget.AllBuffered);
                }
            }
        }

        private void FindPlayerPosition()
        {
            currentCharacterPos = FindClosetPlayerWithForwardPhysic();
            _canAttack = currentCharacterPos;
        }

        [PunRPC]
        private void RpcShot()
        {
            if (CurrentTime <= 0f)
            {
                CarnivorousAttack(0.5f);
                CurrentTime = maxTimeAttack;
            }
        }

        private void CarnivorousAttack(float duration)
        {
            animator.SetTrigger("isAttack");
            DOTween.Sequence()
                .AppendInterval(duration)
                .AppendCallback(() => AttackBullet())
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