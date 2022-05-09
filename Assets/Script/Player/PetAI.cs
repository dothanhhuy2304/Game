using UnityEngine;
using Game.Core;

namespace Game.Player
{

    public class PetAI : BaseObject
    {
        public Data petData;
        private Transform playerPos;
        private Vector2 velocity = Vector2.zero;
        [SerializeField] private FireProjectile[] projectiles;
        [HideInInspector] public GameObject[] multipleEnemy;
        [HideInInspector] public Transform closestEnemy;
        private bool enemyContact;
        [SerializeField] private float distancePlayer;
        [SerializeField] private float rangeAttack;
        private float currentTimeAttack = 3f;
        private const float TimeAttack = 3f;
        private PlayerHealth playerHealth;
        [SerializeField] private Animator animator;
        private readonly AnimationStates animationState = new AnimationStates();

        protected override void Start()
        {
            base.Start();
            playerPos = FindObjectOfType<CharacterController2D>().transform;
            closestEnemy = null;
            enemyContact = false;
            multipleEnemy = GameObject.FindGameObjectsWithTag("Enemy");
            playerHealth = playerPos.GetComponent<PlayerHealth>();
        }

        private void FixedUpdate()
        {
            if (!playerHealth.PlayerIsDeath())
            {
                if (Vector3.Distance(transform.position, playerPos.position) > distancePlayer)
                {
                    Moving();
                    animator.SetBool(animationState.petIsRun, true);
                }
                else
                {
                    body.velocity = Vector2.zero;
                }

                SetTimeAttack(ref currentTimeAttack);
                closestEnemy = FindClosestEnemy();
                if (!enemyContact) return;
                if (Vector2.Distance(transform.position, closestEnemy.position) > rangeAttack) return;
                if (currentTimeAttack != 0f) return;
                Attacks();
                animator.SetBool(animationState.petIsRun, false);
                currentTimeAttack = TimeAttack;
            }
            else
            {
                body.velocity = Vector2.zero;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy")) return;
            closestEnemy.GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.5f);
            enemyContact = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Enemy")) return;
            other.GetComponent<SpriteRenderer>().color = Color.white;
            enemyContact = false;
        }

        private void Moving()
        {
            var angle = (playerPos.transform.position - transform.position).normalized;
            body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref velocity, .05f);
        }

        private void Attacks()
        {
            projectiles[FindBullet()].transform.position = transform.position;
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].SetActives();
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

        private int FindBullet()
        {
            for (var i = 0; i < projectiles.Length; i++)
            {
                if (!projectiles[i].gameObject.activeInHierarchy)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}