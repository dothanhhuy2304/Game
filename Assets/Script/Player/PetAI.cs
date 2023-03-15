using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Player
{

    public class PetAI : FastSingleton<PetAI>
    {
        public Data petData;
        [SerializeField] private Rigidbody2D body;
        private CharacterController2D player;
        private Vector2 velocity = Vector2.zero;
        [SerializeField] private List<FireProjectile> projectiles;
        [HideInInspector] public List<GameObject> multipleEnemy;
        [HideInInspector] public Transform closestEnemy;
        private bool enemyContact;
        [SerializeField] private float distancePlayer;
        [SerializeField] private float rangeAttack;
        private float currentTimeAttack = 3f;
        private const float TimeAttack = 3f;
        [SerializeField] private Animator animator;
        private static readonly int IsRun = Animator.StringToHash("isRun");

        private void Start()
        {
            player = CharacterController2D.instance;
            multipleEnemy = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        }


        private void Update()
        {
            HuyManager.SetTimeAttack(ref currentTimeAttack);
        }

        private void FixedUpdate()
        {
            if (!HuyManager.PlayerIsDeath())
            {
                if (Vector3.Distance(player.transform.position, transform.position) > distancePlayer)
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
                    return;
                }
            }

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
                closestEnemy.GetComponentInChildren<SpriteRenderer>().color = new Color(1f, 0.6f, 0.5f);
                enemyContact = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponentInChildren<SpriteRenderer>().color = Color.white;
                enemyContact = false;
            }
        }

        private void MoveToPlayer()
        {
            Vector2 angle = (player.transform.position - transform.position).normalized;
            body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref velocity, .05f);
        }

        private void BulletAttack()
        {
            projectiles[FindBullet()].transform.position = transform.position;
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].Shoot(transform);
        }

        private Transform FindClosestEnemy()
        {
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in multipleEnemy)
            {
                if (!go) continue;
                float currentDistance = Vector3.Distance(transform.position, go.transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trans = go.transform;
                }
            }

            return trans;
        }

        private int FindBullet()
        {
            for (var i = 0; i < projectiles.Count; i++)
            {
                if (!projectiles[i].gameObject.activeSelf)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}