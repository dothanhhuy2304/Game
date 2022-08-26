using System.Collections.Generic;
using Game.GamePlay;
using UnityEngine;
using Game.Player;

namespace Game.Enemy
{
    public abstract class EnemyController : MonoBehaviour
    {
        [Header("Types")] [SerializeField] protected Rigidbody2D body;
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] protected float movingSpeed;
        protected CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [Space] [Header("Time")] protected float currentTime;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] protected Transform offsetAttack;
        [SerializeField] protected Animator animator;
        [SerializeField] protected EnemyHealth enemyHealth;
        protected bool isRangeAttack;

        protected virtual void Start()
        {
            playerCharacter = CharacterController2D.instance;
        }

        protected void Flip()
        {
            body.velocity = Vector2.zero;
            Vector2 target = (playerCharacter.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg + offsetFlip, 0f));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EvaluateCheckRangeAttack(other, true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            EvaluateCheckRangeAttack(other, false);
        }

        private void EvaluateCheckRangeAttack(Component col, bool canAttack)
        {
            if (col.CompareTag("Player"))
            {
                isRangeAttack = canAttack;
            }
        }

        private static int FindBullet(List<FireProjectile> projectile)
        {
            for (var i = 0; i < projectile.Count; i++)
            {
                if (!projectile[i].gameObject.activeSelf)
                    return i;
            }

            return 0;
        }
        
        protected void AttackBulletDirection()
        {
            Vector2 directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
        public void AttackBulletArc()
        {
            projectiles[FindBullet(projectiles)].transform.rotation = Quaternion.identity;
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
        public void AttackBullet()
        {
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].transform.rotation = transform.rotation;
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

    }
}