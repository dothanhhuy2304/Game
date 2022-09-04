using System;
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
        [SerializeField] protected Vector2 positionAttack;
        [SerializeField] protected Animator animator;
        [SerializeField] protected EnemyHealth enemyHealth;
        protected bool isRangeAttack;
        protected bool isHitGrounds;

        protected virtual void Start()
        {
            playerCharacter = CharacterController2D.instance;
        }

        protected void Flip()
        {
            Vector2 target = (playerCharacter.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg + offsetFlip, 0f));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            EvaluateCheckRangeAttack(other, true);
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGrounds = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            EvaluateCheckRangeAttack(other, false);
            if (other.CompareTag("ground"))
            {
                isHitGrounds = false;
            }
        }


        private void EvaluateCheckRangeAttack(Component col, bool canAttack)
        {
            if (col.CompareTag("Player"))
            {
                isRangeAttack = canAttack;
            }
        }

        protected void AttackBulletDirection()
        {
            Vector2 directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            //projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.position = transform.TransformPoint(positionAttack);
            projectiles[FindBullet()].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            //projectiles[FindBullet(projectiles)].Shoot();
            projectiles[FindBullet()].eventShoot?.Invoke();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
        public void AttackBulletArc()
        {
            //projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.position = transform.TransformPoint(positionAttack);
            projectiles[FindBullet()].transform.rotation = Quaternion.identity;
            //projectiles[FindBullet(projectiles)].Shoot();
            projectiles[FindBullet()].eventShoot?.Invoke();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
        public void AttackBullet()
        {
            //projectiles[FindBullet()].transform.position = offsetAttack.position;
            projectiles[FindBullet()].transform.position = transform.TransformPoint(positionAttack);
            projectiles[FindBullet()].transform.rotation = Quaternion.identity;
            //projectiles[FindBullet(projectiles)].Shoot();
            projectiles[FindBullet()].eventShoot?.Invoke();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }
        
        private int FindBullet()
        {
            for (var i = 0; i < projectiles.Count; i++)
            {
                if (!projectiles[i].gameObject.activeSelf)
                    return i;
            }

            return 0;
        }

    }
}