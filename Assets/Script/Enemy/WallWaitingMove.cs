using UnityEngine;
using Game.Player;

namespace Game.Enemy
{
    public class WallWaitingMove : MonoBehaviour
    {
        private const float Direction = -1f;
        [SerializeField] private float movingSpeed = 2f;
        private Transform player;
        private bool isMoving;

        private void Awake()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                transform.position += Vector3.right * (movingSpeed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("ground"))
            {
                movingSpeed *= Direction;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.collider.gameObject.CompareTag("Player")) return;
            player.transform.parent = transform;
            isMoving = true;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!other.collider.gameObject.CompareTag("Player")) return;
            player.transform.parent = null;
            isMoving = false;
        }
    }
}