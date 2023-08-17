using DG.Tweening;
using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.Enemy
{
    public class Chicken : EnemyController , IPunObservable
    {
        [SerializeField] private float moveTime = 9f;
        [SerializeField] private float moveWaitingTime = 12f;
        [SerializeField] private int[] rotations;
        [SerializeField] private Collider2D chickenCol;
        [SerializeField] private Explosion explosionObj;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool isHitGround;
        private readonly int _isRun = Animator.StringToHash("is_Run");
        private bool _canAttack;

        protected void Start()
        {
            CurrentTime = maxTimeAttack;
            transform.position = enemySetting.startPosition;
            ChickenMoving();
        }

        private void FixedUpdate()
        {
            //photonView.RPC(nameof(RpcPosition), RpcTarget.AllBuffered);
            RpcTargetPosition();

            if (!enemySetting.canAttack)
            {
                if (_canAttack)
                {
                    if ((currentCharacterPos.position - transform.position).magnitude < enemySetting.rangeAttack)
                    {
                        enemySetting.canAttack = true;
                        body.bodyType = RigidbodyType2D.Kinematic;
                    }
                }
            }

            if (enemySetting.canAttack)
            {
                HuyManager.Instance.SetUpTime(ref CurrentTime);
                if ((currentCharacterPos.position - transform.position).magnitude > 0.5f && isHitGround)
                {
                    //photonView.RPC(nameof(MoveToTargets), RpcTarget.AllBuffered, isHitGround);
                    MoveToTargets(isHitGround);
                }
                else
                {
                    //photonView.RPC(nameof(MoveToTargets), RpcTarget.AllBuffered, false);
                    MoveToTargets(false);
                }

                if (CurrentTime <= 0 && spriteRenderer.enabled)
                {
                    if (currentCharacterPos.GetComponent<PlayerHealth>().isDeath)
                    {
                        if ((transform.position - enemySetting.startPosition).magnitude >
                            (transform.position - enemySetting.endPosition).magnitude)
                        {

                            RpcFlip(enemySetting.endPosition, enemySetting.startPosition);
                        }
                        else
                        {
                            RpcFlip(enemySetting.startPosition, enemySetting.endPosition);
                        }

                        enemySetting.canAttack = false;
                        CurrentTime = maxTimeAttack;
                        //photonView.RPC(nameof(ChickenMoving), RpcTarget.AllBuffered);
                    }
                    else
                    {
                        spriteRenderer.enabled = false;
                        chickenCol.enabled = false;
                        explosionObj.transform.position = offsetAttack.position;
                        explosionObj.gameObject.SetActive(true);
                        CurrentTime = maxTimeAttack;
                        enemySetting.canAttack = false;
                        //photonView.RPC(nameof(MoveToTargets), RpcTarget.AllBuffered, false);
                        MoveToTargets(false);
                    }
                }
            }

        }

        //[PunRPC]
        private void RpcTargetPosition()
        {
            currentCharacterPos = FindClosetPlayerWithoutPhysic();
            _canAttack = currentCharacterPos;
        }

        //[PunRPC]
        private void RpcFlip(Vector3 start, Vector3 end)
        {
            Vector3 target = (start - end).normalized;
            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            body.transform.rotation = Quaternion.Euler(new Vector3(0, angle + -45, 0));
        }

        //[PunRPC]
        private void ChickenMoving()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    //photonView.RPC(nameof(MoveToTarget), RpcTarget.AllBuffered, enemySetting.endPosition);
                    MoveToTarget(enemySetting.endPosition);
                })
                .AppendInterval(moveWaitingTime)
                .AppendCallback(() =>
                {
                    //photonView.RPC(nameof(RpcFlip), RpcTarget.AllBuffered, enemySetting.endPosition, enemySetting.startPosition);
                    RpcFlip(enemySetting.endPosition, enemySetting.startPosition);
                })
                .AppendCallback(() =>
                {
                    //photonView.RPC(nameof(MoveToTarget), RpcTarget.AllBuffered, enemySetting.startPosition);
                    MoveToTarget(enemySetting.startPosition);
                })
                .AppendInterval(moveWaitingTime)
                .AppendCallback(() =>
                {
                    //photonView.RPC(nameof(RpcFlip), RpcTarget.AllBuffered, enemySetting.startPosition, enemySetting.endPosition);
                    RpcFlip(enemySetting.startPosition, enemySetting.endPosition);
                })
                .SetLoops(int.MaxValue).Play();
        }

        //[PunRPC]
        private void MoveToTarget(Vector3 target)
        {
            animator.SetBool(_isRun, true);
            body.DOMove(target, moveTime).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    animator.SetBool(_isRun, false);
                });
        }


        //[PunRPC]
        private void MoveToTargets(bool canMove)
        {
            if (canMove)
            {
                Vector3 trans = offsetAttack.transform.position;
                Vector3 movingTarget = (currentCharacterPos.position - trans).normalized;
                Vector3 fixMove = new Vector3(movingTarget.x, 0);
                body.MovePosition(trans + fixMove * (Time.fixedDeltaTime * movingSpeed));
            }
            else
            {
                body.MovePosition(body.position);
            }

            Flip();
            animator.SetBool(_isRun, canMove);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGround = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGround = false;
            }
        }
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((Vector3) body.velocity);
                stream.SendNext((float) body.rotation);
                stream.SendNext((Vector3) transform.position);
                stream.SendNext((Quaternion) transform.rotation);
                stream.SendNext((Vector3) transform.localScale);
            }
            else
            {
                body.velocity = (Vector3) stream.ReceiveNext();
                body.rotation = (float) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
                transform.localScale = (Vector3) stream.ReceiveNext();
            }
        }
    }
}