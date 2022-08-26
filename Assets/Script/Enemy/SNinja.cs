using System.Collections;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class SNinja : EnemyController
    {
        [SerializeField] private Transform rangeAttackObj;
        [SerializeField] private float radiusAttack;
        //
        [Header("SetUp Patrol")] 
        [SerializeField] private Vector3 target;
        [SerializeField] private Vector3[] waypoints;
        [SerializeField] private float timeDurationMoving;
        private static readonly int IsRun = Animator.StringToHash("isRun");
        private static readonly int IsAttackSword = Animator.StringToHash("isAttack1");

        private void FixedUpdate()
        {
            if (enemyHealth.EnemyDeath())
            {
                body.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                body.bodyType = RigidbodyType2D.Kinematic;
                HuyManager.SetTimeAttack(ref currentTime);
                if (!HuyManager.PlayerIsDeath())
                {
                    SNinjaAttack();
                }
                else
                {
                    Move();
                }
            }
        }

        //Moving
        private void Move()
        {
            if (transform.position != target)
            {
                Vector3 position = transform.position;
                Vector3 moveDir = Vector3.MoveTowards(position, target, movingSpeed * Time.fixedDeltaTime);
                body.MovePosition(moveDir);
                animator.SetBool(IsRun, true);
                FaceToWards(target - position);
            }
            else
            {
                if (target == waypoints[0])
                {
                    StartCoroutine(SetTarget(waypoints[1], timeDurationMoving));
                }
                else
                {
                    StartCoroutine(SetTarget(waypoints[0], timeDurationMoving));
                }
            }
        }

        private IEnumerator SetTarget(Vector3 pos, float timeSleep)
        {
            animator.SetBool(IsRun, false);
            yield return new WaitForSeconds(timeSleep);
            target = pos;
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
                Flip();
                SNinjaAttackSword();
            }
            else if ((playerCharacter.transform.position - transform.position).magnitude <= 8)
            {
                Flip();
                animator.SetBool(IsRun, false);
                if (currentTime <= 0f)
                {
                    if (!HuyManager.PlayerIsDeath())
                    {
                        if (!enemyHealth.EnemyDeath())
                        {
                            StartCoroutine(DurationAttack(0.5f));
                        }
                    }

                    currentTime = maxTimeAttack;
                }
            }
            else
            {
                Move();
            }
        }

        private void SNinjaAttackSword()
        {
            bool hits = Physics2D.OverlapCircle(rangeAttackObj.position, radiusAttack, 1 << LayerMask.NameToLayer("Player"));
            if ((playerCharacter.transform.position - transform.position).magnitude >= 2f)
            {
                Vector2 pos = playerCharacter.transform.position - transform.position;
                body.velocity = pos * (35f * Time.fixedDeltaTime);
                animator.SetBool(IsRun, true);
            }
            else if ((playerCharacter.transform.position - transform.position).magnitude < 1.5f)
            {
                animator.SetBool(IsRun, false);
                body.velocity = Vector2.zero;
                if (currentTime <= 0f)
                {
                    animator.SetTrigger(IsAttackSword);
                    currentTime = 1.5f;
                    if (hits)
                    {
                        playerCharacter.playerHealth.GetDamage(21f);
                    }

                    AudioManager.instance.Play("Enemy_Attack_Sword");
                }
            }
        }

        private IEnumerator DurationAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            AttackBulletDirection();
        }

    }
}