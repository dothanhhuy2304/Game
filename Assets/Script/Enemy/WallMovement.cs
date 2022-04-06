using System;
using UnityEngine;
using Game.Player;

namespace Game.Enemy
{
    public class WallMovement : MonoBehaviour
    {
        private const float Direction = -1f;
        public float speed = 3f;
        private Transform player;
        [SerializeField] private MovingInput movingInput;

        private void Awake()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
        }

        private void FixedUpdate()
        {
            switch (movingInput)
            {
                case MovingInput.Horizontal:
                    transform.position += Vector3.left * (speed * Time.deltaTime);
                    break;
                case MovingInput.Vertical:
                    transform.position += Vector3.up * (speed * Time.deltaTime);
                    break;
                case MovingInput.Saw:
                    transform.position += Vector3.up * (speed * Time.deltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            switch (movingInput)
            {
                case MovingInput.Horizontal:
                    if (collision.CompareTag("ground"))
                    {
                        speed *= Direction;
                    }

                    break;
                case MovingInput.Vertical:
                    break;
                case MovingInput.Saw:
                    if (collision.CompareTag("ground"))
                    {
                        speed *= Direction;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            switch (movingInput)
            {
                case MovingInput.Horizontal:
                {
                    if (other.collider.gameObject.CompareTag("Player"))
                    {
                        player.transform.parent = transform;
                    }

                    break;
                }
                case MovingInput.Vertical:
                    break;
                case MovingInput.Saw:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            switch (movingInput)
            {
                case MovingInput.Horizontal:
                    if (other.collider.gameObject.CompareTag("Player"))
                    {
                        player.transform.parent = null;
                    }

                    break;
                case MovingInput.Vertical:
                    if (other.collider.CompareTag("ground"))
                    {
                        speed *= Direction;
                    }

                    break;
                case MovingInput.Saw:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum MovingInput
    {
        Horizontal,
        Vertical,
        Saw
    }
}