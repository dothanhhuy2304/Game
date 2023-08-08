using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;

namespace Script.Player
{

    public class PetAI : FastSingleton<PetAI>
    {
        public Data petData;
        [SerializeField] private Rigidbody2D body;
        private CharacterController2D player;
        //private Vector2 velocity = Vector2.zero;
        [SerializeField] private List<FireProjectile> projectiles;
        private List<GameObject> listEnemyInMap;
        [HideInInspector] public Transform closestEnemy;
        private bool enemyContact;
        [SerializeField] private float distancePlayer;
        [SerializeField] private float rangeAttack;
        private float currentTimeAttack = 3f;
        private const float TimeAttack = 3f;
        [SerializeField] private Animator animator;
        private static readonly int IsRun = Animator.StringToHash("isRun");
        private bool checkHitGround;

        private void Start()
        {
            player = FindObjectOfType<CharacterController2D>();
            listEnemyInMap = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        }


        private void Update()
        {
            HuyManager.Instance.SetUpTime(ref currentTimeAttack);
        }

        private void FixedUpdate()
        {
            if (!HuyManager.Instance.PlayerIsDeath())
            {
                if ((player.transform.position - transform.position).magnitude > distancePlayer)
                {
                    MoveToPlayer();
                    animator.SetBool(IsRun, true);
                    CheckAttack();
                }
                else
                {
                    CheckAttack();
                    body.velocity = Vector2.zero;
                }
            }
        }

        private void CheckAttack()
        {
            closestEnemy = FindClosestEnemy();
            RaycastHit2D hit = Physics2D.Linecast(transform.position, closestEnemy.transform.position, 1 << LayerMask.NameToLayer("ground"));
            if (hit)
            {
                if (hit.collider.CompareTag("ground"))
                {
                    checkHitGround = true;
                    return;
                }
            }

            checkHitGround = false;
            if (enemyContact)
            {
                if (Vector2.Distance(transform.position, closestEnemy.position) < rangeAttack)
                {
                    if (currentTimeAttack <= 0f)
                    {
                        BulletAttack();
                        animator.SetBool(IsRun, false);
                        currentTimeAttack = TimeAttack;
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (!checkHitGround)
                {
                    closestEnemy.GetComponentInChildren<SpriteRenderer>().DOColor(new Color(1f, 0.6f, 0.5f),0.3f);
                    enemyContact = true;
                }
                else
                {
                    closestEnemy.GetComponentInChildren<SpriteRenderer>().DOColor(Color.white, 0.3f);
                    enemyContact = false;
                }
            }
        }
        

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponentInChildren<SpriteRenderer>().DOColor(Color.white, 0.3f);
                enemyContact = false;
            }
        }

        private void MoveToPlayer()
        {
            //Vector2 angle = (player.transform.position - transform.position).normalized;
            //body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref velocity, .05f);
            Vector2 playerPos = player.transform.position;
            body.transform.DOMove(new Vector3(playerPos.x + Random.Range(- 2f , 2f), playerPos.y + 1), 0.5f).SetEase(Ease.Linear);
        }

        private void BulletAttack()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.position;
            projectiles[index].transform.rotation = transform.rotation;
            projectiles[index].Shoot(transform);
        }

        private Transform FindClosestEnemy()
        {
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in listEnemyInMap)
            {
                if (!go) continue;
                float currentDistance = (transform.position - go.transform.position).magnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trans = go.transform;
                }
            }

            return trans;
        }

        private int tempIndex;

        private int FindBullet()
        {
            if (tempIndex >= projectiles.Count - 1)
            {
                return tempIndex = 0;
            }

            tempIndex++;
            if (projectiles[tempIndex].gameObject.activeSelf)
            {
                FindBullet();
            }
            else
            {
                return tempIndex;
            }

            return 0;
        }
    }
}