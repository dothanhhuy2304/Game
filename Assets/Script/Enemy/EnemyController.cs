using System.Collections.Generic;
using UnityEngine;
using Game.Player;

namespace Game.Enemy
{
    //Bug
    public abstract class EnemyController : MonoBehaviour
    {
        [Header("Types")] [SerializeField] protected Rigidbody2D body;
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


        private void EvaluateCheckRangeAttack(Collider2D col, bool canAttack)
        {
            if (col.CompareTag("Player"))
            {
                isRangeAttack = canAttack;
            }
        }
        
        protected static int FindBullet(List<FireProjectile> projectile)
        {
            for (var i = 0; i < projectile.Count; i++)
            {
                if (!projectile[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }

        protected static int FindBullet(List<ProjectileArc> projectileArc)
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