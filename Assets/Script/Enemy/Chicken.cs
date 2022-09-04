using System;
using System.Collections;
using Game.Player;
using UnityEngine;

namespace Game.Enemy
{

    public class Chicken : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D chickenCol;
        [SerializeField] private Explosion explosionObj;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float movingSpeed;
        private bool isMoving;
        private Vector2 startPos = Vector2.zero;
        [Range(0f, 100f)] [SerializeField] protected float rangeAttack = 3f;
        [SerializeField] private float currentTime;
        [SerializeField] private float maxTimeAttack;
        private CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [SerializeField] private Animator animator;
        [SerializeField] private bool isHitGround;
        private static readonly int IsRun = Animator.StringToHash("is_Run");

        private void Start()
        {
            currentTime = maxTimeAttack;
            startPos = transform.position;
            playerCharacter = CharacterController2D.instance;
        }

        private void Update()
        {
            if (isMoving)
            {
                HuyManager.SetTimeAttack(ref currentTime);
            }
        }

        private void FixedUpdate()
        {
            if (HuyManager.PlayerIsDeath())
            {
                StartCoroutine(IEDurationRespawn());
            }
            else
            {
                if (!isMoving)
                {
                    if (Vector3.Distance(transform.position, playerCharacter.transform.position) < rangeAttack)
                    {
                        isMoving = true;
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, playerCharacter.transform.position) > 0.5f)
                    {
                        Flip();
                    }

                    if (currentTime <= 0f)
                    {
                        isMoving = false;
                        MovingToTarget(false);
                        spriteRenderer.enabled = false;
                        chickenCol.enabled = false;
                        animator.enabled = false;
                        explosionObj.transform.position = transform.position;
                        explosionObj.gameObject.SetActive(true);
                        currentTime = maxTimeAttack;
                    }
                    else
                    {
                        MovingToTarget(isHitGround);
                    }
                }
            }
        }
        
        private void Flip()
        {
            Vector2 target = (playerCharacter.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.Euler(new Vector3(0f, Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg + offsetFlip, 0f));
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGround = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                isHitGround = false;
            }
        }

        private void MovingToTarget(bool canMove)
        {
            if (canMove)
            {
                if (Vector2.Distance(playerCharacter.transform.position, transform.position) > 0.5f)
                {
                    body.bodyType = RigidbodyType2D.Kinematic;
                    Vector3 movingTarget = new Vector3(playerCharacter.transform.position.x - transform.position.x, 0f, 0f).normalized;
                    body.MovePosition(body.transform.position + movingTarget * (Time.fixedDeltaTime * movingSpeed));
                }
                else
                {
                    body.bodyType = RigidbodyType2D.Static;
                }
            }
            else
            {
                body.bodyType = RigidbodyType2D.Static;
            }

            animator.SetBool(IsRun, canMove);
        }

        private IEnumerator IEDurationRespawn()
        {
            yield return new WaitForSeconds(4f);
            if (!spriteRenderer.enabled)
            {
                body.bodyType = RigidbodyType2D.Kinematic;
                spriteRenderer.enabled = true;
                chickenCol.enabled = true;
                animator.enabled = true;
                transform.position = startPos;
                MovingToTarget(false);
            }
        }
    }
}