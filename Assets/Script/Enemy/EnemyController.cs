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
        [Header("Types")]
        [SerializeField] protected Rigidbody2D body;
        [SerializeField] protected bool canMoving;
        [SerializeField] protected float movingSpeed;

        [Space] [Header("Prefab")] [SerializeField]
        private FireProjectile[] projectiles;

        [SerializeField] private ProjectileArc[] projectileArcs;
        protected CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [Space] [Header("Time")] protected float currentTime;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] private Transform offsetAttack;
        [SerializeField] protected Animator animator;
        [SerializeField] protected EnemyHealth enemyHealth;

        private void Start()
        {
            playerCharacter = CharacterController2D.instance;
        }

        protected void Flip()
        {
            body.velocity = Vector2.zero;
            var target = (playerCharacter.transform.position - transform.position).normalized;
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
            projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBulletDirection()
        {
            var directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            projectiles[FindBullet()].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBulletArc()
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