using System;
using UnityEngine;
using Game.Player;

namespace Game.Enemy
{
    public class WallWaitingMove : MonoBehaviour
    {
        private const float Direction = -1f;
        private Transform player;
        private bool isMoving;
        [SerializeField] private MovingInput movingInput;
        [SerializeField] private float movingSpeed = 2f;
        private bool isComeback;
        private Vector3 startTrans = Vector3.zero;

        private void Awake()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
            startTrans = transform.position;
        }

        private void FixedUpdate()
        {
            switch (movingInput)
            {
                case MovingInput.Horizontal:
                    if (isMoving)
                    {
                        transform.position += Vector3.right * (movingSpeed * Time.deltaTime);
                    }

                    if (isComeback)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, startTrans, movingSpeed * Time.deltaTime);
                    }

                    break;
                case MovingInput.Vertical:
                    if (isMoving)
                    {
                        transform.position += Vector3.up * (movingSpeed * Time.deltaTime);
                    }

                    if (isComeback)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, startTrans, movingSpeed * Time.deltaTime);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("ground"))
            {
                movingSpeed *= Direction;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.collider.CompareTag("Player")) return;
            player.transform.parent = transform;
            isMoving = true;
            isComeback = false;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = null;
                isMoving = false;
                isComeback = true;

            }
            else if (!other.collider.CompareTag("ground")) return;

            movingSpeed *= Direction;
        }
    }

    public enum MovingInput
    {
        Horizontal,
        Vertical
    }
}