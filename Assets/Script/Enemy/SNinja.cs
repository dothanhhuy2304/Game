using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class SNinja : EnemyController
    {
        [SerializeField] private Transform rangeAttackObj;
        [SerializeField] private float radiusAttack;
        [Space] [SerializeField] private Vector2 checkGroundPosition;
        private CheckEnemyAttack checkEnemyAttack;

        private void Awake()
        {
            checkEnemyAttack = GetComponentInChildren<CheckEnemyAttack>();
        }

        private void FixedUpdate()
        {
            if (playerHealth.PlayerIsDeath())
            {
                enemyHealth.ResetHeathDefault();
            }

            TimeAttack();
            if (enemyHealth.EnemyDeath() || !isVisible)
            {
                body.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                body.bodyType = RigidbodyType2D.Kinematic;


                if (canMoving)
                {
                    var hit = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition), Vector2.down,
                        Distance, 1 << LayerMask.NameToLayer("ground"));
                    var hitRight = Physics2D.Raycast(transform.TransformPoint(checkGroundPosition),
                        Vector2.right,
                        0.5f, 1 << LayerMask.NameToLayer("ground"));
                    if (!hit || hitRight)
                    {
                        transform.Rotate(new Vector3(0, -180f, 0));
                    }

                    Moving(animationState.sNinjaIsRun);
                }

                if (playerHealth.PlayerIsDeath()) return;
                SNinjaAttack();
            }
        }

        private void SNinjaAttack()
        {
            //if (!(Vector3.Distance(transform.position, player.position) <= rangeAttack)) return;
            if (!checkEnemyAttack.canAttack) return;
            if (Vector3.Distance(transform.position, player.position) <= 3f)
            {
                SNinjaAttackSword();
            }
            else
            {
                Flip();
                animator.SetBool(animationState.sNinjaIsRun, false);
                if (currentTime != 0f) return;
                if (playerHealth.PlayerIsDeath()) return;
                Attack();
                currentTime = maxTimeAttack;
            }
        }

        private void SNinjaAttackSword()
        {
            Flip();
            var hits = Physics2D.OverlapCircle(rangeAttackObj.position, radiusAttack,
                1 << LayerMask.NameToLayer("Player"));
            if (Vector3.Distance(transform.position, player.position) >= 2f)
            {
                var pos = player.position - transform.position;
                body.velocity = pos * (35f * Time.fixedDeltaTime);
                animator.SetBool(animationState.sNinjaIsRun, true);
            }
            else
            {
                body.velocity = Vector2.zero;
                animator.SetBool(animationState.sNinjaIsRun, false);
                if (currentTime != 0f) return;
                animator.SetTrigger(animationState.sNinjaIsAttack1);
                currentTime = 1f;
                if (hits)
                {
                    playerHealth.GetDamage(20f);
                }

                PlayerAudio.Instance.Play("Enemy_Attack_Sword");
                //playerAudio.Plays_20("Enemy_Attack_Sword");
            }
        }
    }
}