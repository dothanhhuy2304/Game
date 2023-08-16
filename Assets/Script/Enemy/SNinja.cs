using System;
using System.Collections;
using DG.Tweening;
using Photon.Pun;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class SNinja : EnemyController , IPunObservable
    {
        [SerializeField] private float radiusAttack;
        [Header("SetUp Patrol")]
        [SerializeField] private Vector3 target;

        [SerializeField] private Transform currentTrans;
        [SerializeField] private float timeDurationMoving;
        private static readonly int IsRun = Animator.StringToHash("isRun");
        private static readonly int IsAttackSword = Animator.StringToHash("isAttack1");
        private bool wasAttackSword;
        private bool canAttack;
        [SerializeField] private LayerMask playerMasks;
        [SerializeField] private LayerMask mask;

        protected override void Start()
        {
            base.Start();
            currentTime = 0;
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
            photonView.RPC(nameof(RpcPositionPlayer), RpcTarget.AllBuffered);
            HuyManager.Instance.SetUpTime(ref currentTime);


            if (canAttack)
            {
                if (!HuyManager.Instance.PlayerIsDeath())
                {
                    if (enemySetting.enemyHeal.EnemyDeath())
                    {
                        if (enemySetting.canMoving)
                        {
                            photonView.RPC(nameof(StopMoving), RpcTarget.AllBuffered);
                        }
                    }
                    else
                    {
                        if (!HuyManager.Instance.PlayerIsDeath())
                        {
                            SNinjaAttack();
                        }
                        else
                        {
                            if (enemySetting.canMoving)
                            {
                                photonView.RPC(nameof(MoveToPosition), RpcTarget.AllBuffered);
                            }
                        }
                    }
                }
                else
                {
                    if (enemySetting.canMoving)
                    {
                        photonView.RPC(nameof(MoveToPosition), RpcTarget.AllBuffered);
                    }
                }
            }
            else
            {
                if (enemySetting.canMoving)
                {
                    photonView.RPC(nameof(MoveToPosition), RpcTarget.AllBuffered);
                }
            }
        }

        [PunRPC]
        private void RpcPositionPlayer()
        {
            currentCharacterPos = FindClosestPlayer();

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
        private void StopMoving()
        {
            body.MovePosition(body.transform.position);
        }


        [PunRPC]
        private void MoveToPosition()
        {
            if (transform.position != target)
            {
                Vector3 trans = transform.position;
                Vector3 moveDir = Vector3.MoveTowards(trans, target, movingSpeed * Time.fixedDeltaTime);
                body.MovePosition(moveDir);
                animator.SetBool(IsRun, true);
                FaceToWards(target - trans);
            }
            else
            {
                if (target == enemySetting.startPosition)
                {
                    SetUpTargetToMove(enemySetting.endPosition, timeDurationMoving);
                }
                else
                {
                    SetUpTargetToMove(enemySetting.startPosition, timeDurationMoving);
                }
            }
        }

        private void SetUpTargetToMove(Vector3 pos, float timeSleep)
        {
            animator.SetBool(IsRun, false);
            DOTween.Sequence()
                .AppendInterval(timeSleep)
                .AppendCallback(() =>
                {
                    target = pos;
                }).Play();
        }

        private void FaceToWards(Vector3 direction)
        {
            if (direction.x < 0f)
            {
                transform.localEulerAngles = new Vector3(0, 180f, 0);
            }
            else
            {
                transform.localEulerAngles = Vector3.zero;
            }
        }

        private void SNinjaAttack()
        {
            if ((currentCharacterPos.position - transform.position).magnitude < 3f)
            {
                Flip();
                photonView.RPC(nameof(AttackSword), RpcTarget.AllBuffered);
            }
            else if ((currentCharacterPos.position - transform.position).magnitude >= 3 || (currentCharacterPos.position - transform.position).magnitude <= 8)
            {
                Flip();
                photonView.RPC(nameof(SetUpAttackBullet), RpcTarget.AllBuffered);
            }
            else
            {
                if (enemySetting.canMoving)
                {
                    photonView.RPC(nameof(MoveToPosition), RpcTarget.AllBuffered);
                }
            }
        }

        [PunRPC]
        private void AttackSword()
        {
            if (isHitGrounds)
            {
                if ((currentCharacterPos.position - transform.position).magnitude > 2f)
                {
                    Vector3 currentPosition = body.transform.position;
                    Vector3 targetPosition = new Vector3(currentCharacterPos.position.x - currentPosition.x, 0f, 0f);
                    body.MovePosition(currentPosition + targetPosition * Time.fixedDeltaTime);
                    animator.SetBool(IsRun, true);
                }

                if ((currentCharacterPos.position - transform.position).magnitude < 2f)
                {
                    if (HuyManager.Instance.PlayerIsDeath())
                        return;
                    DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            body.MovePosition(body.transform.position);
                            animator.SetBool(IsRun, false);
                            if (currentTime <= 0f)
                            {
                                wasAttackSword = true;
                                animator.SetTrigger(IsAttackSword);
                                AudioManager.instance.Play("Enemy_Attack_Sword");
                                currentTime = 1.5f;
                            }
                        })
                        .AppendInterval(1f)
                        .AppendCallback(() =>
                        {
                            if (wasAttackSword)
                            {
                                bool hits = Physics2D.OverlapCircle(transform.position, radiusAttack, playerMasks);
                                if (hits)
                                {
                                    currentCharacterPos.GetComponent<CharacterController2D>().playerHealth.RpcGetDamage(21f);
                                }

                                wasAttackSword = false;
                            }
                        }).Play();
                }
            }
        }

        [PunRPC]
        private void SetUpAttackBullet()
        {
            animator.SetBool(IsRun, false);
            body.MovePosition(body.transform.position);
            if (currentTime <= 0f)
            {
                if (!HuyManager.Instance.PlayerIsDeath())
                {
                    if (!enemySetting.enemyHeal.EnemyDeath())
                    {
                        ShootAttack(0.5f);
                    }
                }

                currentTime = maxTimeAttack;
            }
        }

        private void ShootAttack(float durationAttack)
        {
            DOTween.Sequence()
                .AppendInterval(durationAttack)
                .AppendCallback(AttackBulletDirection)
                .Play();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGrounds = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGrounds = false;
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