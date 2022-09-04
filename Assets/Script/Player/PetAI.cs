using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Player
{

    public class PetAI : FastSingleton<PetAI>
    {
        public Data petData;
        [SerializeField] private Rigidbody2D body;
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
        private static readonly int IsRun = Animator.StringToHash("isRun");

        private void Start()
        {
            playerPos = CharacterController2D.instance;
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
                if (Vector3.Distance(playerPos.transform.position, transform.position) > distancePlayer)
                {
                    Moving();
                    animator.SetBool(IsRun, true);
                    Attack();
                }
                else
                {
                    Attack();
                    body.velocity = Vector2.zero;
                }
            }
        }



        private void Attack()
        {
            closestEnemy = FindClosestEnemy();
            if (enemyContact)
            {
                if (Vector2.Distance(transform.position, closestEnemy.position) < rangeAttack)
                {
                    if (currentTimeAttack <= 0f)
                    {
                        Attacks();
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
            projectiles[FindBullet()].transform.position = transform.position;
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].eventShoot?.Invoke();
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