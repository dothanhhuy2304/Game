using System;
using System.Collections.Generic;
using Game.GamePlay;
using Game.Player;
using UnityEngine;

namespace Game.Enemy
{

    [Serializable]
    public class EnemySetting
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public bool isStun;
        public bool canAttack;
        public EnemyHealth enemyHeal;
        public float rangeAttack;
        public bool canMoving;
    }

    public abstract class EnemyController : MonoBehaviour
    {
        public EnemySetting enemySetting;
        [Header("Types")] [SerializeField] protected Rigidbody2D body;
        [SerializeField] protected Animator animator;
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] protected float movingSpeed;
        protected CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [Space] [Header("Time")] protected float currentTime;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] protected Transform offsetAttack;
        [SerializeField] protected Vector2 positionAttack;
        protected bool isHitGrounds;

        protected virtual void Start()
        {
            playerCharacter = CharacterController2D.instance;
        }

        protected void Flip()
        {
            Vector2 target = (playerCharacter.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, angle + offsetFlip, 0));
        }
        
        protected void AttackBulletDirection()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(positionAttack);
            Vector2 direction = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[index].transform.rotation = Quaternion.Euler(0f, 0f, 
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectiles[index].Shoot(transform);
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBullet()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(positionAttack);
            projectiles[index].transform.rotation = Quaternion.identity;
            projectiles[index].Shoot(transform);
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        private int tempIndex;

        private int FindBullet()
        {
            if (tempIndex >= projectiles.Count - 1)
            {
                return tempIndex = 0;
            }

            tempIndex++;
            if (!projectiles[tempIndex].gameObject.activeSelf)
            {
                return tempIndex;
            }

            return 0;
        }

    }
}