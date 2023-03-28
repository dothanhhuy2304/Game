using DG.Tweening;
using UnityEngine;
using Script.Core;

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
        private Vector3 startRotation;
        private static readonly int IsRun = Animator.StringToHash("is_Run");
        private Sequence sequence;

        private void Awake()
        {
            HuyManager.eventResetWhenPlayerDeath += WaitToReset;
        }

        protected override void Start()
        {
            base.Start();
            currentTime = maxTimeAttack;
            transform.position = enemySetting.startPosition;
            startRotation = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
            ChickenMoving();
        }

        private void ChickenMoving()
        {
            sequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    animator.SetBool(IsRun, true);
                    body.DOMove(enemySetting.endPosition, moveTime).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            animator.SetBool(IsRun, false);
                        });
                }).AppendInterval(moveWaitingTime)
                .AppendCallback(() =>
                {
                    body.transform.DORotate(new Vector3(0, rotations[0], 0), 0);
                })
                .AppendCallback(() =>
                {
                    animator.SetBool(IsRun, true);
                    body.DOMove(enemySetting.startPosition, moveTime).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            animator.SetBool(IsRun, false);
                        });
                }).AppendInterval(moveWaitingTime)
                .AppendCallback(() =>
                {
                    body.transform.DORotate(new Vector3(0, rotations[1], 0), 0);
                })
                .SetLoops(int.MaxValue).Play();
        }

        private void Update()
        {
            if (enemySetting.canAttack)
            {
                sequence.Kill();
                HuyManager.SetTimeAttack(ref currentTime);
            }
            
        }

        private void FixedUpdate()
        {
            if (!enemySetting.canAttack)
            {
                if ((playerCharacter.transform.position - transform.position).magnitude < enemySetting.rangeAttack)
                {
                    enemySetting.canAttack = true;
                    body.bodyType = RigidbodyType2D.Kinematic;
                }
            }

            if (enemySetting.canAttack)
            {
                if ((playerCharacter.transform.position - transform.position).magnitude > 0.5f && isHitGround)
                {
                    MoveToTarget(isHitGround);
                }
                else
                {
                    MoveToTarget(false);
                }

                if (currentTime <= 0f)
                {
                    enemySetting.canAttack = false;
                    MoveToTarget(false);
                    spriteRenderer.enabled = false;
                    chickenCol.enabled = false;
                    animator.enabled = false;
                    explosionObj.transform.position = offsetAttack.position;
                    explosionObj.gameObject.SetActive(true);
                    currentTime = maxTimeAttack;
                }
            }
        }

        private void MoveToTarget(bool canMove)
        {
            if (canMove)
            {
                Vector3 trans = offsetAttack.transform.position;
                Vector3 movingTarget = (playerCharacter.transform.position - trans).normalized;
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

        private void WaitToReset()
        {
            if (HuyManager.PlayerIsDeath())
            {
                DOTween.Sequence()
                    .AppendCallback(() => { body.MovePosition(body.transform.position); }).AppendInterval(4f)
                    .AppendCallback(() =>
                    {
                        enemySetting.canAttack = false;
                        spriteRenderer.enabled = true;
                        chickenCol.enabled = true;
                        animator.enabled = true;
                        transform.position = enemySetting.startPosition;
                        transform.Rotate(new Vector3(0, rotations[0], 0));
                        gameObject.SetActive(true);
                        ChickenMoving();
                    }).Play();
            }
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