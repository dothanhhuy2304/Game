using System;
using System.Collections;
using UnityEngine;
using Game.Player;
using Game.Core;

namespace Game.Enemy
{
    //Bug
    public class EnemyController : BaseObject
    {
        [SerializeField] private float movingSpeed = 2f;
        [Header("Types")] [SerializeField] private EnemyType enemyType;

        [Space] [Header("Prefab")] [SerializeField]
        private GameObject[] bulletHolder;

        [Space] private Transform player;
        [SerializeField] private Vector2 checkGroundPosition;
        [SerializeField] private Transform rangeAttackObj;
        private const float Distance = 1.5f;
        [Range(0f, 100f)] [SerializeField] private float rangeAttack = 7f;
        [SerializeField] private float offset;
        [SerializeField] private float radiusAttack;

        [Header("CarnivorousPlant")] [SerializeField]
        private float currentTime;

        [SerializeField] private float maxTimeAttack = 1f;
        [SerializeField] private Vector2 offsetAttack;
        [SerializeField] private AudioClip swordAudio;
        private PlayerHealth playerHealth;
        private Animator animator;
        private EnemyHealth enemyHealth;
        private readonly AnimationStates animationState = new AnimationStates();
        private PlayerAudio playerAudio;

        protected override void Start()
        {
            base.Start();
            player = FindObjectOfType<CharacterController2D>().transform;
            playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
            Debug.Assert(player != null, nameof(player) + " != null");
            playerHealth = player.GetComponent<PlayerHealth>();
            currentTime = 0f;
            animator = GetComponent<Animator>();
            enemyHealth = GetComponent<EnemyHealth>();
        }

        protected override void FixedUpdate()
        {
            if (enemyHealth.EnemyDeath())
            {
                body.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                body.bodyType = RigidbodyType2D.Kinematic;
                switch (enemyType)
                {
                    case EnemyType.SNINJA:
                    {
                        var hit = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition), Vector2.down,
                            Distance, 1 << LayerMask.NameToLayer("ground"));
                        var hitRight = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition),
                            Vector2.right,
                            0.5f, 1 << LayerMask.NameToLayer("ground"));
                        if (!hit || hitRight)
                        {
                            transform.Rotate(new Vector3(0, -180f, 0));
                        }

                        Moving();
                        SNinjaAttack();
                        break;
                    }
                    case EnemyType.CarnivorousPlant:
                    {
                        if (Vector3.Distance(transform.position, player.position) < rangeAttack)
                        {
                            Flip();
                            if (SetTimeAttack(ref currentTime) != 0f) return;
                            animator.SetTrigger(animationState.carnivorousPlantIsAttack);
                            currentTime = maxTimeAttack;
                            Attack();
                        }

                        break;
                    }
                    case EnemyType.Player:
                        break;
                    case EnemyType.Pet:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Moving()
        {
            body.velocity = body.transform.right * movingSpeed;
            animator.SetBool(animationState.sNinjaIsRun, true);
        }

        private void SNinjaAttack()
        {
            if (CheckDistance(transform.position, player.transform.position) > 20f) return;
            if (playerHealth.PlayerIsDeath()) return;
            if (!(Vector3.Distance(transform.position, player.position) <= rangeAttack)) return;
            if (Vector3.Distance(transform.position, player.position) <= 3f)
            {
                Flip();
                var hits = Physics2D.OverlapCircle(rangeAttackObj.position, radiusAttack,
                    1 << LayerMask.NameToLayer("Player"));
                if (Vector3.Distance(transform.position, player.position) >= 2f)
                {
                    var pos = player.position - transform.position;
                    body.velocity = pos * (35f * Time.fixedDeltaTime);
                    animator.SetBool(animationState.sNinjaIsRun, true);
                }
                else
                {
                    body.velocity = Vector2.zero;
                    animator.SetBool(animationState.sNinjaIsRun, false);
                    if (SetTimeAttack(ref currentTime) != 0f) return;
                    animator.SetTrigger(animationState.sNinjaIsAttack1);
                    currentTime = 1f;
                    if (hits)
                    {
                        playerHealth.GetDamage(20f);
                    }

                    playerAudio.Plays_20("Enemy_Attack_Sword");
                }
            }
            else
            {
                Flip();
                animator.SetBool(animationState.sNinjaIsRun, false);
                if (SetTimeAttack(ref currentTime) != 0f) return;
                currentTime = maxTimeAttack;
                Attack();
            }
        }

        private void Flip()
        {
            body.velocity = Vector2.zero;
            Vector2 target = (player.position - transform.position).normalized;
            var angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, angle + offset, 0f));
            //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        private void Attack()
        {
            StartCoroutine(nameof(EnumeratorAttack), .6f);
        }


        private IEnumerator EnumeratorAttack(float timeDelay)
        {
            switch (enemyType)
            {
                case EnemyType.SNINJA:
                {
                    yield return new WaitForSeconds(timeDelay);
                    AttackNinja();
                    break;
                }
                case EnemyType.CarnivorousPlant:
                {
                    yield return new WaitForSeconds(timeDelay);
                    AttackCarnivorousPlant();
                    break;
                }
                case EnemyType.Player:
                    break;
                case EnemyType.Pet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AttackCarnivorousPlant()
        {
            bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offsetAttack);
            bulletHolder[FindBullet()].transform.rotation = transform.rotation;
            bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
            playerAudio.Plays_20("Enemy_Attack_Shoot");
            //Instantiate(prefab, transform.TransformPoint(offsetAttack), transform.rotation);
        }

        private void AttackNinja()
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