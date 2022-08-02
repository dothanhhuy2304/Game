using Game.Core;
using UnityEngine;

namespace Game.Enemy
{

    public class Chicken : EnemyController
    {
        [SerializeField] private Collider2D chickenCol;
        [SerializeField] private GameObject explosionObj;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private bool isMoving;
        private bool destroy;
        private Vector2 startPos = Vector2.zero;
        private float timeRespawn;
        [SerializeField] private Vector2 groundCheck = Vector2.zero;
        [Range(0f, 100f)] [SerializeField] protected float rangeAttack = 3f;
        private bool isRespawn;

        private void Awake()
        {
            currentTime = maxTimeAttack;
            startPos = transform.position;
            timeRespawn = 4f;
        }

        private void OnEnable()
        {
            chickenCol.enabled = true;
        }

        private void FixedUpdate()
        {
            if (HuyManager.PlayerIsDeath())
            {
                isRespawn = true;
            }

            if (isRespawn)
            {
                if (!spriteRenderer.enabled)
                {
                    BaseObject.SetTimeAttack(ref timeRespawn);
                    if (timeRespawn != 0) return;
                    body.bodyType = RigidbodyType2D.Kinematic;
                    destroy = false;
                    spriteRenderer.enabled = true;
                    chickenCol.enabled = true;
                    animator.enabled = true;
                    transform.position = startPos;
                    MovingToTarget("is_Run", false);
                    timeRespawn = 4f;
                }
                else
                {
                    isRespawn = false;
                }
            }

            if (destroy) return;
            if (!isMoving)
            {
                if (Vector3.Distance(transform.position, playerCharacter.transform.position) < rangeAttack)
                {
                    isMoving = true;
                }
            }
            else
            {
                var hit = Physics2D.Raycast(transform.TransformPoint(groundCheck), Vector3.down, 2f,
                    1 << LayerMask.NameToLayer("ground"));
                var hitRight = Physics.Raycast(transform.TransformPoint(groundCheck), Vector3.zero, 0f,
                    1 << LayerMask.NameToLayer("ground"));
                BaseObject.SetTimeAttack(ref currentTime);
                if (Vector3.Distance(transform.position, playerCharacter.transform.position) > 0.5f)
                {
                    Flip();
                }

                if (!hit || hitRight)
                {
                    body.velocity = Vector3.zero;
                    animator.SetBool("is_Run", false);
                }
                else
                {
                    MovingToTarget("is_Run", true);
                }

                if (currentTime != 0f) return;
                body.bodyType = RigidbodyType2D.Static;
                destroy = true;
                isMoving = false;
                spriteRenderer.enabled = false;
                chickenCol.enabled = false;
                animator.enabled = false;
                explosionObj.transform.position = transform.position;
                explosionObj.SetActive(true);
                currentTime = maxTimeAttack;
            }
        }

        private void MovingToTarget(string states, bool value)
        {
            var target = new Vector3(playerCharacter.transform.position.x - transform.position.x, 0f, 0f).normalized;
            if (Vector2.Distance(playerCharacter.transform.position, transform.position) > 1f)
            {
                body.MovePosition(body.transform.position + target * (movingSpeed * Time.fixedDeltaTime));
            }
            else
            {
                body.velocity = Vector2.zero;
            }

            animator.SetBool(states, value);
        }
    }
}