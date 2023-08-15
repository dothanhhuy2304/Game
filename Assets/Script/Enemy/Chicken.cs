using DG.Tweening;
using Photon.Pun;
using Script.Core;
using UnityEngine;

namespace Script.Enemy
{
    public class Chicken : EnemyController
    {
        [SerializeField] private float moveTime = 9f;
        [SerializeField] private float moveWaitingTime = 12f;
        [SerializeField] private int[] rotations;
        [SerializeField] private Collider2D chickenCol;
        [SerializeField] private Explosion explosionObj;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool isHitGround;
        private Quaternion startRotation;
        private static readonly int IsRun = Animator.StringToHash("is_Run");

        protected void Awake()
        {
            HuyManager.Instance.eventResetWhenPlayerDeath += WaitToReset;
            startRotation = transform.rotation;
        }

        protected override void Start()
        {
            base.Start();
            currentTime = maxTimeAttack;
            transform.position = enemySetting.startPosition;
            ChickenMoving();
        }

        private void WaitToReset()
        {
            if (!spriteRenderer.enabled)
            {
                photonView.RPC(nameof(RpcEnemyReSpawn), RpcTarget.All);
            }
        }

        private void RpcEnemyReSpawn()
        {
            DOTween.Sequence()
                .AppendCallback(() => { body.MovePosition(body.transform.position); })
                .AppendInterval(3f)
                .AppendCallback(() =>
                {
                    currentTime = maxTimeAttack;
                    enemySetting.canAttack = false;
                    spriteRenderer.enabled = true;
                    chickenCol.enabled = true;
                    Transform trans = transform;
                    trans.rotation = startRotation;
                    trans.position = enemySetting.startPosition;
                    transform.Rotate(new Vector3(0, rotations[1], 0));
                    gameObject.SetActive(true);
                    ChickenMoving();
                }).Play();
        }

        private void FixedUpdate()
        {
            if (enemySetting.canAttack)
            {
                HuyManager.Instance.SetUpTime(ref currentTime);
            }

            photonView.RPC(nameof(RpcPosition), RpcTarget.All);
            if (!enemySetting.canAttack)
            {
                if ((currentCharacterPos.position - transform.position).magnitude < enemySetting.rangeAttack)
                {
                    enemySetting.canAttack = true;
                    body.bodyType = RigidbodyType2D.Kinematic;
                }
            }

            if (enemySetting.canAttack && spriteRenderer.enabled)
            {
                if ((currentCharacterPos.position - transform.position).magnitude > 0.5f && isHitGround)
                {
                    photonView.RPC(nameof(MoveToTarget), RpcTarget.All, isHitGround);
                }
                else
                {
                    photonView.RPC(nameof(MoveToTarget), RpcTarget.All, false);
                }

                if (currentTime <= 0f)
                {
                    if (HuyManager.Instance.PlayerIsDeath())
                    {
                        if ((transform.position - enemySetting.startPosition).magnitude >
                            (transform.position - enemySetting.endPosition).magnitude)
                        {

                            photonView.RPC(nameof(RpcFlip), RpcTarget.All, enemySetting.endPosition, enemySetting.startPosition);
                        }
                        else
                        {
                            photonView.RPC(nameof(RpcFlip), RpcTarget.All, enemySetting.startPosition, enemySetting.endPosition);
                        }

                        enemySetting.canAttack = false;
                        currentTime = maxTimeAttack;
                        //ChickenMoving();
                    }
                    else
                    {
                        enemySetting.canAttack = false;
                        photonView.RPC(nameof(MoveToTarget), RpcTarget.All, false);
                        spriteRenderer.enabled = false;
                        chickenCol.enabled = false;
                        explosionObj.transform.position = offsetAttack.position;
                        explosionObj.gameObject.SetActive(true);
                        currentTime = maxTimeAttack;
                    }
                }
            }
        }

        [PunRPC]
        private void RpcPosition()
        {
            currentCharacterPos = FindClosetPlayerWithoutPhysic();
        }

        [PunRPC]
        private void RpcFlip(Vector3 start, Vector3 end)
        {
            Vector3 target = (start - end).normalized;
            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            body.transform.rotation = Quaternion.Euler(new Vector3(0, angle + -45, 0));
        }

        [PunRPC]
        private void ChickenMoving()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    photonView.RPC(nameof(MoveToTarget), RpcTarget.All, enemySetting.endPosition);
                })
                .AppendInterval(moveWaitingTime)
                .AppendCallback(() =>
                {
                    photonView.RPC(nameof(RpcFlip), RpcTarget.All, enemySetting.endPosition,
                        enemySetting.startPosition);
                })
                .AppendCallback(() =>
                {
                    photonView.RPC(nameof(MoveToTarget), RpcTarget.All, enemySetting.startPosition);
                })
                .AppendInterval(moveWaitingTime)
                .AppendCallback(() =>
                {
                    photonView.RPC(nameof(RpcFlip), RpcTarget.All, enemySetting.startPosition,
                        enemySetting.endPosition);
                })
                .SetLoops(int.MaxValue).Play();
        }

        [PunRPC]
        private void MoveToTarget(Vector3 target)
        {
            animator.SetBool(IsRun, true);
            body.DOMove(target, moveTime).SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    animator.SetBool(IsRun, false);
                });
        }


        [PunRPC]
        private void MoveToTarget(bool canMove)
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
                body.MovePosition(body.transform.position);
            }

            Flip();
            animator.SetBool(IsRun, canMove);
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
    }
}