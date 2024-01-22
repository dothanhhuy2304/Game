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
        private readonly int _isAttack = Animator.StringToHash("isAttack");

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
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            currentCharacterPos = FindClosetPlayerWithForwardPhysic();
            _canAttack = currentCharacterPos;
        }

        [PunRPC]
        private void RpcShot()
        {
            if (CurrentTime <= 0f)
            {
                CarnivorousAttack();
                CurrentTime = maxTimeAttack;
            }
        }

        private void CarnivorousAttack()
        {
            animator.SetTrigger(_isAttack);
            DOTween.Sequence()
                .AppendInterval(0.5f)
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