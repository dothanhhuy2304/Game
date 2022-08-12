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
        [SerializeField] protected EnemyManager enemyManager;
        protected bool isRangeAttack;

        protected virtual void Start()
        {
            playerCharacter = CharacterController2D.instance;
            enemyManager = EnemyManager.instance;
        }

        protected void Flip()
        {
            body.velocity = Vector2.zero;
            Vector2 target = (playerCharacter.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg + offsetFlip, 0f));
        }

        // protected static bool CheckAttack(Vector2 point, Vector2 size)
        // {
        //     return Physics2D.OverlapBox(point, size, 0f, 1 << LayerMask.NameToLayer("Player"));
        // }

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
    }
}