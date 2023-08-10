using DG.Tweening;
using UnityEngine;
using Script.Core;
using Script.Player;

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
        private Sequence sequence;

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
                    //body.transform.DORotate(new Vector3(0, rotations[0], 0), 0);
                    Vector2 target = (enemySetting.endPosition - enemySetting.startPosition).normalized;
                    float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
                    body.transform.rotation = Quaternion.Euler(new Vector3(0, angle + -45, 0));
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
                    //body.transform.DORotate(new Vector3(0, rotations[1], 0), 0);
                    Vector2 target = (enemySetting.startPosition - enemySetting.endPosition).normalized;
                    float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
                    body.transform.rotation = Quaternion.Euler(new Vector3(0, angle + -45, 0));
                })
                .SetLoops(int.MaxValue).Play();
        }

        private void Update()
        {
            if (enemySetting.canAttack)
            {
                sequence.Kill();
                HuyManager.Instance.SetUpTime(ref currentTime);
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

            if (enemySetting.canAttack && spriteRenderer.enabled)
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
                    if (HuyManager.Instance.PlayerIsDeath())
                    {
                        if ((transform.position - enemySetting.startPosition).magnitude > (transform.position - enemySetting.endPosition).magnitude)
                        {
                            Vector2 target = (enemySetting.endPosition - enemySetting.startPosition).normalized;
                            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
                            body.transform.rotation = Quaternion.Euler(new Vector3(0, angle + -45, 0));
                        }
                        else
                        {
                            Vector2 target = (enemySetting.startPosition - enemySetting.endPosition).normalized;
                            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
                            body.transform.rotation = Quaternion.Euler(new Vector3(0, angle + -45, 0));
                        }

                        enemySetting.canAttack = false;
                        currentTime = maxTimeAttack;
                        ChickenMoving();
                    }
                    else
                    {
                        enemySetting.canAttack = false;
                        MoveToTarget(false);
                        spriteRenderer.enabled = false;
                        chickenCol.enabled = false;
                        explosionObj.transform.position = offsetAttack.position;
                        explosionObj.gameObject.SetActive(true);
                        currentTime = maxTimeAttack;
                    }
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