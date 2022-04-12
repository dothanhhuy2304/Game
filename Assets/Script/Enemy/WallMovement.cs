using System;
using Game.Core;
using UnityEngine;
using Game.Player;

//Improver
namespace Game.Enemy
{
    public class WallMovement : BaseObject
    {
        private const float Direction = -1f;
        public float speed = 3f;
        private Transform player;
        [SerializeField] private MovingInput movingInput;
        private Vector3 startPos = Vector3.zero;

        protected override void Start()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
            startPos = transform.position;
        }

        protected override void FixedUpdate()
        {
            if (base.CheckDistance(transform.position, player.transform.position) > 30f &&
                transform.position == startPos)
            {
                var transform1 = transform;
                transform1.position = transform1.position;
            }
            else
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
        }

        private void OnTriggerEnter2D(Collider2D collision)
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
                    if (other.collider.CompareTag("Player"))
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
                    if (other.collider.CompareTag("Player"))
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