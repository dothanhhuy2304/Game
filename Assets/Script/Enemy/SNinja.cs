using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class SNinja : EnemyController
    {
        [SerializeField] private float radiusAttack;

        [Header("SetUp Patrol")] 
        [SerializeField] private Vector3 target;

        private readonly int _isRun = Animator.StringToHash("isRun");
        private readonly int _isAttackSword = Animator.StringToHash("isAttack1");
        private bool _canAttackSword;
        private bool _canAttack;
        private bool _isHitGrounds;

        private void FixedUpdate()
        {
            if (enemySetting.enemyHeal.EnemyDeath())
            {
                return;
            }

            FindPlayerPosition();
            if (_canAttack && !currentCharacterPos.GetComponent<PlayerHealth>().isDeath)
            {
                HuyManager.Instance.SetUpTime(ref currentTime);
                if ((currentCharacterPos.position - transform.position).magnitude <= 3f)
                {
                    if (View.IsMine)
                    {
                        Flip();
                    }

                    View.RPC(nameof(AttackSword), RpcTarget.AllBuffered);
                }
                else if ((currentCharacterPos.position - transform.position).magnitude > 3 &&
                         (currentCharacterPos.position - transform.position).magnitude <= 8)
                {
                    if (View.IsMine)
                    {
                        Flip();
                    }

                    View.RPC(nameof(RpcAttackBullet), RpcTarget.AllBuffered);

                }
                else
                {
                    if (View.IsMine)
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
                if (View.IsMine)
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
            Transform objectTrans = transform;
            if (objectTrans.position != target)
            {
                animator.SetBool(_isRun, true);
                body.MovePosition(Vector3.MoveTowards(transform.position, target, movingSpeed * Time.deltaTime));
                objectTrans.localEulerAngles = (target - objectTrans.position).x < 0f ? new Vector3(0, 180f, 0) : Vector3.zero;
            }
            else
            {
                animator.SetBool(_isRun, false);
                SetUpTargetToMove(target == enemySetting.startPosition
                    ? enemySetting.endPosition
                    : enemySetting.startPosition);
            }
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
            if (!_isHitGrounds)
            {
                return;
            }

            if ((currentCharacterPos.position - transform.position).magnitude > 2f)
            {
                Vector2 currentPosition = body.position;
                Vector2 targetPosition = new Vector2(currentCharacterPos.position.x - currentPosition.x, 0f);
                body.MovePosition(currentPosition + targetPosition * Time.fixedDeltaTime);
                animator.SetBool(_isRun, true);
            }
            else
            {
                animator.SetBool(_isRun, false);
                if (currentTime <= 0f)
                {
                    _canAttackSword = true;
                    animator.SetTrigger(_isAttackSword);
                    AudioManager.instance.Play("Enemy_Attack_Sword");
                    currentTime = 1.5f;
                }

                DOTween.Sequence()
                    .AppendInterval(1.5f)
                    .AppendCallback(() =>
                    {
                        if (_canAttackSword)
                        {
                            var hits = Physics2D.OverlapCircleAll(transform.position, radiusAttack, LayerMaskManager.instance.onlyPlayerMask);
                            if (hits.Length > 0)
                            {
                                hits.ToList().ForEach(t =>
                                {
                                    PlayerHealth playerHealth = t.GetComponent<PlayerHealth>();
                                    if (!playerHealth.isDeath)
                                    {
                                        playerHealth.GetDamage(21);
                                    }
                                });

                            }
                        }

                        _canAttackSword = false;
                    }).Play();
            }
        }

        [PunRPC]
        private void RpcAttackBullet()
        {
            animator.SetBool(_isRun, false);
            if (currentTime <= 0f)
            {
                DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(() => AttackBullet(true))
                    .Play();
                currentTime = maxTimeAttack;
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
                stream.SendNext((bool) _canAttack);
                stream.SendNext((Vector3) body.velocity);
                stream.SendNext((float) body.rotation);
                stream.SendNext((Vector3) transform.position);
                stream.SendNext((Quaternion) transform.rotation);
                stream.SendNext((Vector3) transform.localEulerAngles);
            }
            else
            {
                _canAttack = (bool) stream.ReceiveNext();
                body.velocity = (Vector3) stream.ReceiveNext();
                body.rotation = (float) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
                transform.localEulerAngles = (Vector3) stream.ReceiveNext();
            }
        }

    }
}