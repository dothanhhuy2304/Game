using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Core;

namespace Game.Player
{

    public class PetAI : FastSingleton<PetAI>
    {
        public Data petData;
        private Rigidbody2D body;
        private CharacterController2D playerPos;
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

        private void Start()
        {
            body = GetComponent<Rigidbody2D>();
            playerPos = CharacterController2D.instance;
            multipleEnemy = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        }

        private void FixedUpdate()
        {
            if (!HuyManager.PlayerIsDeath())
            {
                if (Vector3.Distance(transform.position, playerPos.transform.position) > distancePlayer)
                {
                    Moving();
                    animator.SetBool("isRun", true);
                }
                else
                {
                    body.velocity = Vector2.zero;
                }

                HuyManager.SetTimeAttack(ref currentTimeAttack);
                closestEnemy = FindClosestEnemy();
                if (enemyContact)
                {
                    if (Vector2.Distance(transform.position, closestEnemy.position) < rangeAttack)
                    {
                        if (currentTimeAttack <= 0f)
                        {
                            Attacks();
                            animator.SetBool("isRun", false);
                            currentTimeAttack = TimeAttack;
                        }
                        else
                        {
                            body.velocity = Vector2.zero;
                        }
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                closestEnemy.GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.5f);
                enemyContact = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponent<SpriteRenderer>().color = Color.white;
                enemyContact = false;
            }
        }

        private void Moving()
        {
            Vector2 angle = (playerPos.transform.position - transform.position).normalized;
            body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref velocity, .05f);
        }

        private void Attacks()
        {
            projectiles[FindBullet(projectiles)].transform.position = transform.position;
            projectiles[FindBullet(projectiles)].transform.rotation = transform.rotation;
            projectiles[FindBullet(projectiles)].Shoot();
        }

        private Transform FindClosestEnemy()
        {
            var closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in multipleEnemy)
            {
                if (!go) continue;
                var currentDistance = Vector3.Distance(transform.position, go.transform.position);
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trans = go.transform;
                }
            }

            return trans;
        }

        private int FindBullet(List<FireProjectile> projectile)
        {
            for (var i = 0; i < projectile.Count; i++)
            {
                if (!projectile[i].gameObject.activeInHierarchy)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}