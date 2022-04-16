using Game.Core;
using UnityEngine;
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
        private Vector3 startTrans = Vector3.zero;

        protected override void Start()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
            startTrans = transform.position;
        }

        protected override void FixedUpdate()
        {
            if (isMoving)
            {
                transform.position += Vector3.right * (movingSpeed * Time.deltaTime);
            }

            if (isComeback)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, startTrans, movingSpeed * Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("ground"))
            {
                movingSpeed *= Direction;
            }
            else if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = transform;
                isMoving = true;
                isComeback = false;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = null;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;
            if (!other.CompareTag("ground")) return;
            isComeback = true;
            isMoving = false;
        }
    }
}