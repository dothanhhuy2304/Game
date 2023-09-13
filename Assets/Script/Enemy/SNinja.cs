using DG.Tweening;
using Photon.Pun;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class SNinja : EnemyController, IPunObservable
    {
        [SerializeField] private float radiusAttack;

        [Header("SetUp Patrol")] [SerializeField]
        private Vector3 target;

        private readonly int _isRun = Animator.StringToHash("isRun");
        private readonly int _isAttackSword = Animator.StringToHash("isAttack1");
        private bool _canAttackSword;
        private bool _canAttack;
        private bool _isHitGrounds;

        [SerializeField] private LayerMask playerMasks;

        private void Awake()
        {
            if (pv == null)
            {
                pv = GetComponent<PhotonView>();
            }

            CurrentTime = maxTimeAttack;
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
                if (!currentCharacterPos.GetComponent<PlayerHealth>().isDeath)
                {
                    if ((currentCharacterPos.position - transform.position).magnitude <= 3f)
                    {
                        if (pv.IsMine)
                        {
                            Flip();
                        }

                        pv.RPC(nameof(AttackSword), RpcTarget.AllBuffered);
                    }
                    else if ((currentCharacterPos.position - transform.position).magnitude > 3 && (currentCharacterPos.position - transform.position).magnitude <= 8)
                    {
                        if (pv.IsMine)
                        {
                            Flip();
                        }

                        pv.RPC(nameof(RpcAttackBullet), RpcTarget.AllBuffered);
                        
                    }
                    else
                    {
                        if (pv.IsMine)
                        {
                            if (enemySetting.canMoving)
                            {
                                MoveToPosition();
                            }
                        }

                    }
                }
                else
                {
                    if (pv.IsMine)
                    {
                        if (enemySetting.canMoving)
                        {
                            MoveToPosition();
                        }
                    }
                }
            }
            else
            {
                if (pv.IsMine)
                {
                    if (enemySetting.canMoving)
                    {
                        MoveToPosition();
                    }
                }
            }
        }

        private void FindPlayerPosition()
        {
            currentCharacterPos = FindClosestPlayer();
            _canAttack = currentCharacterPos;

        }

        private void MoveToPosition()
        {
            Transform obj = transform;
            if (obj.position != target)
            {
                pv.RPC(nameof(RpcNinjaMove), RpcTarget.AllBuffered);
                obj.localEulerAngles = (target - obj.position).x < 0f ? new Vector3(0, 180f, 0) : Vector3.zero;

            }
            else
            {
                pv.RPC(nameof(RpcNinjaMove2), RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void RpcNinjaMove()
        {
            animator.SetBool(_isRun, true);
            body.MovePosition(Vector3.MoveTowards(transform.position, target, movingSpeed * Time.fixedDeltaTime));
        }

        [PunRPC]
        private void RpcNinjaMove2()
        {
            animator.SetBool(_isRun, false);
            SetUpTargetToMove(target == enemySetting.startPosition
                ? enemySetting.endPosition
                : enemySetting.startPosition);
        }

        private void SetUpTargetToMove(Vector3 location)
        {
            DOTween.Sequence()
                .AppendInterval(2f)
                .AppendCallback(() => { target = location; })
                .Play();
        }

        [PunRPC]
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
                else
                {
                    body.MovePosition(body.position);
                    animator.SetBool(_isRun, false);
                    if (CurrentTime <= 0f)
                    {
                        _canAttackSword = true;
                        animator.SetTrigger(_isAttackSword);
                        AudioManager.instance.Play("Enemy_Attack_Sword");
                        CurrentTime = 1.5f;
                    }

                    DOTween.Sequence()
                        .AppendInterval(1f)
                        .AppendCallback(() =>
                        {
                            if (_canAttackSword)
                            {
                                bool hitPlayer = Physics2D.OverlapCircle(transform.position, radiusAttack, playerMasks);
                                if (hitPlayer)
                                {
                                    currentCharacterPos.GetComponent<PlayerHealth>().RpcGetDamage(21f);
                                }

                                _canAttackSword = false;
                            }
                        }).Play();
                }
            }
        }

        [PunRPC]
        private void RpcAttackBullet()
        {
            body.MovePosition(body.position);
            animator.SetBool(_isRun, false);
            if (CurrentTime <= 0f)
            {
                DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(() => AttackBullet(true))
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
                stream.SendNext((Vector3) transform.localEulerAngles);
            }
            else
            {
                body.velocity = (Vector3) stream.ReceiveNext();
                body.rotation = (float) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
                transform.localEulerAngles = (Vector3) stream.ReceiveNext();
            }
        }

    }
}