using System;
using UnityEngine;

namespace Game.Enemy
{

    public class Chicken : EnemyController
    {
        [SerializeField] private Collider2D chickenCol;
        [SerializeField] private GameObject explosionObj;
        private SpriteRenderer spriteRenderer;
        private bool isMoving;
        private bool destroy;
        private Vector2 startPos = Vector2.zero;
        private float timeRespawn;
        [SerializeField] private Vector2 groundCheck = Vector2.zero;
        [Range(0f, 100f)] [SerializeField] protected float rangeAttack = 3f;
        private bool isRespawn;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (playerHealth.PlayerIsDeath())
            {
                isRespawn = true;
            }

            if (isRespawn)
            {
                if (!spriteRenderer.enabled)
                {
                    SetTimeAttack(ref timeRespawn);
                    if (timeRespawn != 0) return;
                    body.bodyType = RigidbodyType2D.Kinematic;
                    destroy = false;
                    spriteRenderer.enabled = true;
                    chickenCol.enabled = true;
                    animator.enabled = true;
                    transform.position = startPos;
                    MovingToTarget(animationState.chickenIsAttack, false);
                    timeRespawn = 4f;
                }
                else
                {
                    isRespawn = false;
                }
            }

            if (!isVisible) return;
            if (destroy) return;
            if (!isMoving)
            {
                if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
                isMoving = true;
            }
            else
            {
                var hit = Physics2D.Raycast(transform.TransformPoint(groundCheck), Vector3.down, 2f,
                    1 << LayerMask.NameToLayer("ground"));
                var hitRight = Physics.Raycast(transform.TransformPoint(groundCheck), Vector3.zero, 0f,
                    1 << LayerMask.NameToLayer("ground"));
                SetTimeAttack(ref currentTime);
                if (Vector3.Distance(transform.position, player.position) > 0.5f)
                {
                    Flip();
                }

                if (!hit || hitRight)
                {
                    body.velocity = Vector3.zero;
                    animator.SetBool(animationState.chickenIsAttack, false);
                }
                else
                {
                    MovingToTarget(animationState.chickenIsAttack, true);
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

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawSphere(transform.position,rangeAttack);
        // }
    }

    // private void OnDrawGizmos()
    // {
    // Gizmos.DrawRay(transform.TransformPoint(groundCheck), Vector3.down * 2f);
    // Gizmos.DrawSphere(transform.position, rangeAttack);
    // }
}