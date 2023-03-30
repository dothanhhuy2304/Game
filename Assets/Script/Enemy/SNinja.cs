using DG.Tweening;
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

        [SerializeField] private Transform currentTrans;
        [SerializeField] private float timeDurationMoving;
        private static readonly int IsRun = Animator.StringToHash("isRun");
        private static readonly int IsAttackSword = Animator.StringToHash("isAttack1");
        private bool wasAttackSword;
        private bool canAttack;
        [SerializeField] private LayerMask mask;

        private void Awake()
        {
            currentTime = 0;
            HuyManager.eventResetWhenPlayerDeath += WaitToReset;
        }

        private void WaitToReset()
        {
            if (HuyManager.PlayerIsDeath())
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
            HuyManager.SetUpTime(ref currentTime);
            RaycastHit2D hit = Physics2D.Linecast(transform.position, playerCharacter.transform.position, mask);
            if (hit)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    canAttack = true;
                }
                else if (hit.collider.CompareTag("ground"))
                {
                    canAttack = false;
                }
            }

            if (canAttack)
            {
                if (!HuyManager.PlayerIsDeath())
                {
                    if (enemySetting.enemyHeal.EnemyDeath())
                    {
                        if (enemySetting.canMoving)
                        {
                            body.MovePosition(body.transform.position);
                        }
                    }
                    else
                    {
                        if (!HuyManager.PlayerIsDeath())
                        {
                            SNinjaAttack();
                        }
                        else
                        {
                            if (enemySetting.canMoving)
                            {
                                DOTween.Sequence()
                                    .AppendInterval(0.5f)
                                    .AppendCallback(MoveToPosition).Play();
                            }
                        }
                    }
                }
                else
                {
                    if (enemySetting.canMoving)
                    {
                        DOTween.Sequence()
                            .AppendInterval(0.5f)
                            .AppendCallback(MoveToPosition).Play();
                    }
                }
            }
            else
            {
                if (enemySetting.canMoving)
                {
                    DOTween.Sequence()
                        .AppendInterval(0.3f)
                        .AppendCallback(MoveToPosition).Play();
                }
            }
        }

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
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    animator.SetBool(IsRun, false);
                }).AppendInterval(timeSleep)
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
            if ((playerCharacter.transform.position - transform.position).magnitude < 3f)
            {
                AttackSword();
            }
            else if ((playerCharacter.transform.position - transform.position).magnitude <= 8)
            {
                SetUpAttackBullet();
            }
            else
            {
                if (enemySetting.canMoving)
                {
                    MoveToPosition();
                }
            }
        }

        private void AttackSword()
        {
            Flip();
            if (isHitGrounds)
            {
                if ((playerCharacter.transform.position - transform.position).magnitude > 2f)
                {
                    Vector3 currentPosition = body.transform.position;
                    Vector3 targetPosition = new Vector3(playerCharacter.transform.position.x - currentPosition.x, 0f, 0f);
                    body.MovePosition(currentPosition + targetPosition * Time.fixedDeltaTime);
                    animator.SetBool(IsRun, true);
                }

                if ((playerCharacter.transform.position - transform.position).magnitude < 2f)
                {
                    if (HuyManager.PlayerIsDeath())
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
                                bool hits = Physics2D.OverlapCircle(transform.position, radiusAttack, 1 << LayerMask.NameToLayer("Player"));
                                if (hits)
                                {
                                    playerCharacter.playerHealth.GetDamage(21f);
                                }

                                wasAttackSword = false;
                            }
                        }).Play();
                }
            }
        }

        private void SetUpAttackBullet()
        {
            Flip();
            animator.SetBool(IsRun, false);
            body.MovePosition(body.transform.position);
            if (currentTime <= 0f)
            {
                if (!HuyManager.PlayerIsDeath())
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

    }
}