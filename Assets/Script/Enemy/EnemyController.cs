using System;
using System.Collections;
using UnityEngine;
using Game.Player;
using Game.Core;

namespace Game.Enemy
{
    //Bug
    public abstract class EnemyController : BaseObject
    {
        [Header("Types")] [SerializeField] protected EnemyType enemyType;
        [SerializeField] protected bool canMoving;
        [SerializeField] private float movingSpeed = 2f;

        [Space] [Header("Prefab")] [SerializeField]
        private GameObject[] bulletHolder;

        protected Transform player;
        protected const float Distance = 1.5f;
        [Range(0f, 100f)] [SerializeField] protected float rangeAttack = 7f;
        [SerializeField] private float offsetFlip;
        protected float currentTime;

        [SerializeField] protected float maxTimeAttack;
        [SerializeField] private Vector2 offsetAttack = Vector2.zero;
        protected PlayerHealth playerHealth;
        protected Animator animator;
        protected EnemyHealth enemyHealth;
        protected readonly AnimationStates animationState = new AnimationStates();
        protected PlayerAudio playerAudio;

        protected override void Start()
        {
            base.Start();
            player = FindObjectOfType<CharacterController2D>().transform;
            playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
            playerHealth = player.GetComponent<PlayerHealth>();
            animator = GetComponent<Animator>();
            enemyHealth = GetComponent<EnemyHealth>();
        }

        //protected virtual void FixedUpdate()
        //{
            //SetTimeAttack(ref currentTime);
            //base.CheckDistance(player.position, transform.position);
            // switch (enemyType)
            // {
            //     case EnemyType.SNINJA:
            //     {
            // if (enemyHealth.EnemyDeath())
            // {
            //     body.bodyType = RigidbodyType2D.Static;
            // }
            // else
            // {
            //     body.bodyType = RigidbodyType2D.Kinematic;
            //
            //
            //     if (canMoving)
            //     {
            //         var hit = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition), Vector2.down,
            //             Distance, 1 << LayerMask.NameToLayer("ground"));
            //         var hitRight = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition),
            //             Vector2.right,
            //             0.5f, 1 << LayerMask.NameToLayer("ground"));
            //         if (!hit || hitRight)
            //         {
            //             transform.Rotate(new Vector3(0, -180f, 0));
            //         }
            //
            //         Moving();
            //     }
            //
            //     if (playerHealth.PlayerIsDeath()) return;
            //     if (hasInteracted)
            //     {
            //         SNinjaAttack();
            //     }
            // }

            //     break;
            // }
            // case EnemyType.CarnivorousPlant:
            // {
            // if(playerHealth.PlayerIsDeath()) return;
            // if (enemyHealth.EnemyDeath()) return;
            // if (Vector3.Distance(transform.position, player.position) < rangeAttack)
            // {
            //     Flip();
            //     if (currentTime != 0f) return;
            //     animator.SetTrigger(animationState.carnivorousPlantIsAttack);
            //     currentTime = maxTimeAttack;
            //     Attack();
            // }

            //         break;
            //     }
            //     case EnemyType.Player:
            //         break;
            //     case EnemyType.Pet:
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        //}

        protected void CheckUpdate()
        {
            SetTimeAttack(ref currentTime);
            base.CheckDistance(player.position, transform.position);
        }

        protected void Moving(int states)
        {
            body.velocity = body.transform.right * movingSpeed;
            animator.SetBool(states, true);
        }

        protected void MovingToTarget(int states,bool value)
        {
            var target = new Vector3(player.position.x - transform.position.x, 0f, 0f).normalized;
            body.MovePosition(body.transform.position + target * (movingSpeed * Time.fixedDeltaTime));
            animator.SetBool(states, value);
        }

        protected void Flip()
        {
            body.velocity = Vector2.zero;
            Vector2 target = (player.position - transform.position).normalized;
            var angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, angle + offsetFlip, 0f));
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        protected void Attack()
        {
            if (!playerHealth.PlayerIsDeath())
            {
                StartCoroutine(nameof(EnumeratorAttack), .6f);
            }
            else
            {
                StopCoroutine(nameof(EnumeratorAttack));
            }
        }

        private IEnumerator EnumeratorAttack(float timeDelay)
        {
            switch (enemyType)
            {
                case EnemyType.SNINJA:
                {
                    yield return new WaitForSeconds(timeDelay);
                    AttackBulletDirection();
                    break;
                }
                case EnemyType.CarnivorousPlant:
                {
                    yield return new WaitForSeconds(timeDelay);
                    AttackBullet();
                    break;
                }
                case EnemyType.Bee:
                    yield return new WaitForSeconds(timeDelay);
                    AttackBulletDirection();
                    break;
                case EnemyType.Player:
                    break;
                case EnemyType.Pet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AttackBullet()
        {
            bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offsetAttack);
            bulletHolder[FindBullet()].transform.rotation = transform.rotation;
            bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
            playerAudio.Plays_20("Enemy_Attack_Shoot");
            //Instantiate(prefab, transform.TransformPoint(offsetAttack), transform.rotation);
        }

        private void AttackBulletDirection()
        {
            var directionVector = (player.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(Vector3.forward, directionVector);
            bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offsetAttack);
            bulletHolder[FindBullet()].transform.rotation = Quaternion.Euler(0f, 0f, lookRotation.eulerAngles.z + 90f);
            bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
            playerAudio.Plays_20("Enemy_Attack_Shoot");
            //Instantiate(prefab, transform.TransformPoint(offsetAttack), Quaternion.Euler(transform.rotation.x, transform.rotation.y, lookRotation.eulerAngles.z + 90));
        }

        private int FindBullet()
        {
            for (var i = 0; i < bulletHolder.Length; i++)
            {
                if (!bulletHolder[i].activeInHierarchy)
                    return i;
            }

            return 0;
        }

        //void OnDrawGizmos()
        //{
        //  Gizmos.DrawSphere(rangeAttackObj.position, radiusAttack);
        //Gizmos.DrawSphere(transform.position, 2f);
        //}
    }
}