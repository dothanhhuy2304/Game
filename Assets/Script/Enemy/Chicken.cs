using DG.Tweening;
using Game.Player;
using UnityEngine;

namespace Game.Enemy
{
    public class Chicken : EnemyController
    {
        [SerializeField] private Collider2D chickenCol;
        [SerializeField] private Explosion explosionObj;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [Range(0f, 100f)] [SerializeField] protected float rangeAttack = 3f;
        [SerializeField] private bool isHitGround;
        private static readonly int IsRun = Animator.StringToHash("is_Run");
        private Sequence sequence;
        protected override void Start()
        {
            currentTime = maxTimeAttack;
            transform.position = enemySetting.startPosition;
            playerCharacter = CharacterController2D.instance;
            ChickenMoving();
        }

        private void ChickenMoving()
        {
            sequence = DOTween.Sequence()
                .AppendCallback(() =>
                {
                    animator.SetBool(IsRun, true);
                    body.DOMove(enemySetting.endPosition, 9).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            animator.SetBool(IsRun, false);
                        });
                }).AppendInterval(12f)
                .AppendCallback(() =>
                {
                    FlipMoving(spriteRenderer, true);
                })
                .AppendCallback(() =>
                {
                    animator.SetBool(IsRun, true);
                    body.DOMove(enemySetting.startPosition, 9).SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            animator.SetBool(IsRun, false);
                        });
                }).AppendInterval(12)
                .AppendCallback(() =>
                {
                    FlipMoving(spriteRenderer, false);
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
            if (HuyManager.PlayerIsDeath())
            {
                WaitToSpawn();
            }
            else
            {
                if (!enemySetting.canAttack)
                {
                    if ((playerCharacter.transform.position - transform.position).magnitude < rangeAttack)
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
        }

        private void MoveToTarget(bool canMove)
        {
            if (canMove)
            {
                spriteRenderer.flipX = false;
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

        private void WaitToSpawn()
        {
            DOTween.Sequence()
                .AppendCallback(() => { body.MovePosition(body.transform.position); }).AppendInterval(4f)
                .AppendCallback(() =>
                {
                    spriteRenderer.enabled = true;
                    chickenCol.enabled = true;
                    animator.enabled = true;
                    transform.position = enemySetting.startPosition;
                    MoveToTarget(false);
                }).Play();
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