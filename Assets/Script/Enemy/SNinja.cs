using Game.Core;
using Game.GamePlay;
using UnityEngine;

namespace Game.Enemy
{
    public class SNinja : EnemyController
    {
        [SerializeField] private Transform rangeAttackObj;
        private const float Distance = 1.5f;
        [SerializeField] private float radiusAttack;
        [Space] [SerializeField] private Vector2 checkGroundPosition;
        [SerializeField] private Vector2 posAttack = Vector2.zero;
        [SerializeField] private Vector2 rangerAttack = Vector2.zero;

        private void FixedUpdate()
        {
            if (HuyManager.PlayerIsDeath())
            {
                enemyHealth.ResetHeathDefault();
            }

            if (enemyHealth.EnemyDeath())
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

                    Moving("isRun");
                }

                if (HuyManager.PlayerIsDeath()) return;
                BaseObject.SetTimeAttack(ref currentTime);
                SNinjaAttack();
            }
        }

        private void Moving(string states)
        {
            body.velocity = body.transform.right * movingSpeed;
            animator.SetBool(states, true);
        }


        private void SNinjaAttack()
        {
            if (!CheckAttack(transform.position + (Vector3) posAttack, rangerAttack)) return;
            if (Vector3.Distance(transform.position, playerCharacter.transform.position) <= 3f)
            {
                SNinjaAttackSword();
            }
            else
            {
                Flip();
                animator.SetBool("isRun", false);
                if (HuyManager.PlayerIsDeath()) return;
                if (currentTime != 0f) return;
                if (!HuyManager.PlayerIsDeath() || !enemyHealth.EnemyDeath())
                {
                    StartCoroutine(DurationAttack(0.5f));
                }

                currentTime = maxTimeAttack;
            }
        }

        private void SNinjaAttackSword()
        {
            Flip();
            var hits = Physics2D.OverlapCircle(rangeAttackObj.position, radiusAttack,
                1 << LayerMask.NameToLayer("Player"));
            if (Vector3.Distance(transform.position, playerCharacter.transform.position) >= 2f)
            {
                var pos = playerCharacter.transform.position - transform.position;
                body.velocity = pos * (35f * Time.fixedDeltaTime);
                animator.SetBool("isRun", true);
            }
            else
            {
                body.velocity = Vector2.zero;
                animator.SetBool("isRun", false);
                if (currentTime != 0f) return;
                animator.SetTrigger("isAttack1");
                currentTime = 1.5f;
                if (hits)
                {
                    playerCharacter.playerHealth.GetDamage(21f);
                }

                AudioManager.instance.Play("Enemy_Attack_Sword");
            }
        }

        private System.Collections.IEnumerator DurationAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            AttackBulletDirection();
        }

    }
}