using System;
using UnityEngine;
using Game.Core;
using Game.Player;

namespace Game.Enemy
{
    public class WallWaitingMove : BaseObject
    {
        private const float Direction = -1f;
        private Transform player;
        private bool isMoving;
        [SerializeField] private float movingSpeed = 2f;
        private bool isComeback;
        private bool playerExit;
        private Vector3 startTrans = Vector3.zero;

        protected override void Start()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
            startTrans = transform.position;
        }

        private void Update()
        {
            base.CheckDistance(player.position, transform.position);
            if (!hasInteracted) return;
            if (!hasInteracted)
            {
                isComeback = true;
            }

            if (isMoving)
            {
                transform.position += Vector3.right * (movingSpeed * Time.deltaTime);
            }

            if (isComeback)
            {
                transform.position = Vector2.MoveTowards(transform.position, startTrans, 2f * Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("ground"))
            {
                if (!playerExit)
                {
                    movingSpeed *= Direction;
                }
                else
                {
                    movingSpeed *= Direction;
                    isComeback = true;
                }
            }

            if (!other.collider.CompareTag("Player")) return;
            player.transform.parent = transform;
            isMoving = true;
            isComeback = false;
            playerExit = false;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!other.collider.CompareTag("Player")) return;
            player.transform.parent = null;
            playerExit = true;
        }
    }
}