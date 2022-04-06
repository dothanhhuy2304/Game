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
        private Vector3 posAwake;
        private FireProjectile fireProjectile;
        private readonly AnimationStates animationState = new AnimationStates();

        public override void Awake()
        {
            base.Awake();
            player = FindObjectOfType<CharacterController2D>().transform;
            Debug.Assert(player != null, nameof(player) + " != null");
            playerHealth = player.GetComponent<PlayerHealth>();
            currentTime = 0f;
            animator = GetComponent<Animator>();
            enemyHealth = gameObject.GetComponent<EnemyHealth>();
            fireProjectile = bulletHolder[FindBullet()].GetComponent<FireProjectile>();
            posAwake = transform.position;
        }

        private void FixedUpdate()
        {
            if (enemyHealth.EnemyDeath())
            {
                transform.position = posAwake;
            }
            else
            {
                switch (enemyType)
                {
                    case EnemyType.SNINJA:
                    {
                        var hit = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition), Vector2.down,
                            Distance, 1 << LayerMask.NameToLayer("ground"));
                        var hitRight = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition), Vector2.right,
                            0.5f, 1 << LayerMask.NameToLayer("ground"));
                        if (!hit || hitRight)
                        {
                            transform.Rotate(new Vector3(0, -180f, 0));
                        }

                        Moving();
                        break;
                    }
                    case EnemyType.CarnivorousPlant:
                    {
                        if (Vector3.Distance(transform.position, player.position) < rangeAttack)
                        {
                            Flip();
                            currentTime -= Time.deltaTime;
                            if (currentTime <= 0)
                            {
                                animator.SetTrigger(animationState.carnivorousPlantIsAttack);
                                currentTime = maxTimeAttack;
                                Attack();
                            }
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
            if (Vector3.Distance(transform.position, player.position) >= rangeAttack || playerHealth.PlayerIsDeath())
            {
                body.velocity = body.transform.right * movingSpeed;
                animator.SetBool(animationState.sNinjaIsRun, true);
            }
            else if (!playerHealth.PlayerIsDeath())
            {
                if (currentTime != 0f)
                {
                    currentTime -= Time.deltaTime;
                }

                if (!(Vector3.Distance(transform.position, player.position) <= rangeAttack)) return;
                if (Vector3.Distance(transform.position, player.position) <= 3f)
                {
                    Flip();
                    if (Vector3.Distance(transform.position, player.position) <= 3f &&
                        Vector3.Distance(transform.position, player.position) > 2f)
                    {
                        var position = transform.position;
                        var pos = new Vector3(player.position.x - position.x, position.y);
                        body.velocity = pos;
                        animator.SetBool(animationState.sNinjaIsRun, true);
                    }

                    if (!(currentTime <= 0)) return;
                    var hits = new Collider2D[10];
                    if (Physics2D.OverlapCircleNonAlloc(transform.TransformPoint(checkGroundPosition), radiusAttack,
                        hits, 1 << LayerMask.NameToLayer("Player")) <= 0) return;
                    foreach (var hit in hits)
                    {
                        try
                        {
                            animator.SetBool(animationState.sNinjaIsRun, false);
                            body.velocity = Vector2.zero;
                            animator.SetBool(animationState.sNinjaIsRun, true);
                            if (!hit.CompareTag("Player") &&
                                !(Vector3.Distance(transform.position, player.position) <= 3f)) continue;
                            animator.SetTrigger(animationState.sNinjaIsAttack1);
                            currentTime = 1f;
                            playerHealth.GetDamage(20f);
                            AudioSource.PlayClipAtPoint(swordAudio, transform.position, 1f);
                        }
                        catch (Exception e)
                        {
                            Debug.Assert(e != null, e.Message);
                            return;
                        }
                    }
                }
                else
                {
                    animator.SetBool(animationState.sNinjaIsRun, false);
                    Flip();
                    if (!(currentTime <= 0f)) return;
                    currentTime = maxTimeAttack;
                    Attack();
                }
            }
        }

        private void Flip()
        {
            body.velocity = Vector2.zero;
            Vector2 target = (player.position - transform.position).normalized;
            var angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, angle + offset, 0f));
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
                    this.AttackNinja();
                    break;
                }
                case EnemyType.CarnivorousPlant:
                {
                    yield return new WaitForSeconds(timeDelay);
                    this.AttackCarnivorousPlant();
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
            //bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offsetAttack);
            //bulletHolder[FindBullet()].transform.rotation = transform.rotation;
            var trans = fireProjectile.transform;
            var currentsPos = transform;
            trans.position = currentsPos.TransformPoint(offsetAttack);
            trans.rotation = currentsPos.rotation;
            fireProjectile.SetActives();
            //Instantiate(prefab, transform.TransformPoint(offsetAttack), transform.rotation);
        }

        private void AttackNinja()
        {
            var directionVector = (player.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(Vector3.forward, directionVector);
            fireProjectile.transform.position = transform.TransformPoint(offsetAttack);
            fireProjectile.transform.rotation = Quaternion.Euler(0f, 0f, lookRotation.eulerAngles.z + 90f);
            fireProjectile.SetActives();
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

        // void OnDrawGizmos()
        // {
        //     Gizmos.DrawSphere(transform.TransformPoint(checkGroundPosition), radiusAttack);
        // }
    }
}