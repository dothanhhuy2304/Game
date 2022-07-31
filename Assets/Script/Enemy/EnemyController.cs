using System;
using UnityEngine;
using Game.Player;
using Game.Core;
using Game.GamePlay;

namespace Game.Enemy
{
    //Bug
    public abstract class EnemyController : MonoBehaviour
    {
        [Header("Types")] [SerializeField] protected EnemyType enemyType;
        public Rigidbody2D body;
        [SerializeField] protected bool canMoving;
        [SerializeField] private float movingSpeed;

        [Space] [Header("Prefab")] [SerializeField]
        private FireProjectile[] projectiles;

        [SerializeField] private ProjectileArc[] projectileArcs;
        public CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [Space] [Header("Time")] protected float currentTime;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] private Transform offsetAttack;
        [SerializeField] protected Animator animator;
        [SerializeField] protected EnemyHealth enemyHealth;

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
            playerCharacter = CharacterController2D.instance;
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

        // protected void TimeAttack()
        // {
        //     SetTimeAttack(ref currentTime);
        // }

        protected void Moving(string states)
        {
            body.velocity = body.transform.right * movingSpeed;
            animator.SetBool(states, true);
        }

        protected void MovingToTarget(string states, bool value)
        {
            var target = new Vector3(playerCharacter.transform.position.x - transform.position.x, 0f, 0f).normalized;
            if (Vector2.Distance(playerCharacter.transform.position, transform.position) > 1f)
            {
                body.MovePosition(body.transform.position + target * (movingSpeed * Time.fixedDeltaTime));
            }
            else
            {
                body.velocity = Vector2.zero;
            }

            animator.SetBool(states, value);
        }

        protected void Flip()
        {
            if (body)
            {
                body.velocity = Vector2.zero;
            }

            var target = (playerCharacter.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg + offsetFlip, 0f));
        }

        protected static bool CheckAttack(Vector2 point, Vector2 size)
        {
            return Physics2D.OverlapBox(point, size, 0f, 1 << LayerMask.NameToLayer("Player"));
        }

        protected void Attack()
        {
            if (HuyManager.PlayerIsDeath() || enemyHealth.EnemyDeath()) return;
            StartCoroutine(DurationAttack(0.5f));
        }

        private System.Collections.IEnumerator DurationAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            Attacks();
        }


        private void Attacks()
        {
            switch (enemyType)
            {
                case EnemyType.SNINJA:
                {
                    AttackBulletDirection();
                    break;
                }
                case EnemyType.CarnivorousPlant:
                {
                    AttackBullet();
                    break;
                }
                case EnemyType.Bee:
                    AttackBulletDirection();
                    break;
                case EnemyType.Trunk:
                    AttackBulletArc();
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
            projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        private void AttackBulletDirection()
        {
            var directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            projectiles[FindBullet()].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        private void AttackBulletArc()
        {
            projectileArcs[FindBulletArc()].transform.rotation = Quaternion.identity;
            projectileArcs[FindBulletArc()].transform.position = offsetAttack.position;
            projectileArcs[FindBulletArc()].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        private int FindBullet()
        {
            for (var i = 0; i < projectiles.Length; i++)
            {
                if (!projectiles[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }

        private int FindBulletArc()
        {
            for (var i = 0; i < projectileArcs.Length; i++)
            {
                if (!projectileArcs[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }
    }
}