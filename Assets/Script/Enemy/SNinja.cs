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

        [SerializeField] private float timeDurationMoving;
        private readonly int _isRun = Animator.StringToHash("isRun");
        private readonly int _isAttackSword = Animator.StringToHash("isAttack1");
        private bool _wasAttackSword;
        private bool _canAttack;
        private bool _isHitGrounds;

        [SerializeField] private LayerMask playerMasks;

        private void FixedUpdate()
        {
            RpcTargetPosition();
            HuyManager.Instance.SetUpTime(ref CurrentTime);

            if (_canAttack)
            {
                if (!currentCharacterPos.GetComponent<PlayerHealth>().isDeath)
                {
                    if (!enemySetting.enemyHeal.EnemyDeath())
                    {
                        if ((currentCharacterPos.position - transform.position).magnitude <= 3f)
                        {
                            Flip();
                            AttackSword();
                        }
                        else if ((currentCharacterPos.position - transform.position).magnitude > 3 && (currentCharacterPos.position - transform.position).magnitude <= 8)
                        {
                            Debug.LogError("pos");
                            Flip();
                            RpcAttackBullet();
                        }
                        else
                        {
                            if (enemySetting.canMoving)
                            {
                                MoveToPosition();
                            }
                        }
                    }
                    else
                    {
                        if (enemySetting.canMoving)
                        {
                            StopMoving();
                        }
                    }
                }
                else
                {
                    if (enemySetting.canMoving)
                    {
                        MoveToPosition();
                    }
                }
            }
            else
            {
                if (enemySetting.canMoving)
                {
                    MoveToPosition();
                }
            }
        }

        private void RpcTargetPosition()
        {
            currentCharacterPos = FindClosestPlayer();
            _canAttack = currentCharacterPos != null;
        }

        private void StopMoving()
        {
            body.MovePosition(body.position);
        }

        private void MoveToPosition()
        {
            if (transform.position != target)
            {
                Vector3 trans = transform.position;
                Vector3 moveDir = Vector3.MoveTowards(trans, target, movingSpeed * Time.fixedDeltaTime);
                body.MovePosition(moveDir);
                animator.SetBool(_isRun, true);
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
            animator.SetBool(_isRun, false);
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

        private void AttackSword()
        {
            if (_isHitGrounds)
            {
                if ((currentCharacterPos.position - transform.position).magnitude > 2f)
                {
                    Vector3 currentPosition = body.position;
                    Vector3 targetPosition = new Vector3(currentCharacterPos.position.x - currentPosition.x, 0f, 0f);
                    body.MovePosition(currentPosition + targetPosition * Time.fixedDeltaTime);
                    animator.SetBool(_isRun, true);
                }
                
                if ((currentCharacterPos.position - transform.position).magnitude < 2f)
                {
                    body.MovePosition(body.position);
                    animator.SetBool(_isRun, false);
                    if (CurrentTime <= 0f)
                    {
                        _wasAttackSword = true;
                        animator.SetTrigger(_isAttackSword);
                        AudioManager.instance.Play("Enemy_Attack_Sword");
                        CurrentTime = 1.5f;
                    }

                    DOTween.Sequence()
                        .AppendInterval(1f)
                        .AppendCallback(() =>
                        {
                            if (_wasAttackSword)
                            {
                                bool hitPlayer = Physics2D.OverlapCircle(transform.position, radiusAttack, playerMasks);
                                if (hitPlayer)
                                {
                                    currentCharacterPos.GetComponent<PlayerHealth>()?.RpcGetDamage(21f);
                                }

                                _wasAttackSword = false;
                            }
                        }).Play();
                }
            }
        }

        private void RpcAttackBullet()
        {
            animator.SetBool(_isRun, false);
            body.MovePosition(body.position);
            Debug.LogError(CurrentTime);
            if (CurrentTime <= 0f)
            {
                DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(AttackBulletDirection)
                    .Play();
                CurrentTime = maxTimeAttack;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                _isHitGrounds = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                _isHitGrounds = false;
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