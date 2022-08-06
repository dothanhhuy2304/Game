using System.Collections.Generic;
using UnityEngine;
using Game.Player;
using Game.GamePlay;

namespace Game.Enemy
{
    //Bug
    public abstract class EnemyController : MonoBehaviour
    {
        [Header("Types")]
        [SerializeField] protected Rigidbody2D body;
        [SerializeField] protected float movingSpeed;
        [Space] [Header("Prefab")] 
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] private List<ProjectileArc> projectileArcs;
        protected CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [Space] [Header("Time")] protected float currentTime;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] private Transform offsetAttack;
        [SerializeField] protected Animator animator;
        [SerializeField] protected EnemyHealth enemyHealth;

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

        protected static bool CheckAttack(Vector2 point, Vector2 size)
        {
            return Physics2D.OverlapBox(point, size, 0f, 1 << LayerMask.NameToLayer("Player"));
        }
        
        // private void Attack()
        // {
        //     if (HuyManager.PlayerIsDeath() || enemyHealth.EnemyDeath()) return;
        //     StartCoroutine(DurationAttack(0.5f));
        // }
        //
        // private System.Collections.IEnumerator DurationAttack(float duration)
        // {
        //     yield return new WaitForSeconds(duration);
        //     Attacks();
        // }
        //
        // private void Attacks()
        // {
        //     switch (enemyType)
        //     {
        //         case EnemyType.SNINJA:
        //         {
        //             AttackBulletDirection();
        //             break;
        //         }
        //         case EnemyType.CarnivorousPlant:
        //         {
        //             AttackBullet();
        //             break;
        //         }
        //         case EnemyType.Bee:
        //             AttackBulletDirection();
        //             break;
        //         case EnemyType.Trunk:
        //             AttackBulletArc();
        //             break;
        //         case EnemyType.Player:
        //             break;
        //         case EnemyType.Pet:
        //             break;
        //     }
        // }

        protected void AttackBullet()
        {
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].transform.rotation = transform.rotation;
            projectiles[FindBullet(projectiles)].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBulletDirection()
        {
            Vector2 directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack.position;
            projectiles[FindBullet(projectiles)].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            projectiles[FindBullet(projectiles)].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBulletArc()
        {
            projectileArcs[FindBullet(projectileArcs)].transform.rotation = Quaternion.identity;
            projectileArcs[FindBullet(projectileArcs)].transform.position = offsetAttack.position;
            projectileArcs[FindBullet(projectileArcs)].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        private static int FindBullet(List<FireProjectile> projectile)
        {
            for (var i = 0; i < projectile.Count; i++)
            {
                if (!projectile[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }

        private static int FindBullet(List<ProjectileArc> projectileArc)
        {
            for (var i = 0; i < projectileArc.Count; i++)
            {
                if (!projectileArc[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }
    }
}