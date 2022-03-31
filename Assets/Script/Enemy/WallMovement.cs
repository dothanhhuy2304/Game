using UnityEngine;
using Game.Player;

namespace Game.Enemy
{
    public class WallMovement : MonoBehaviour
    {
        private const float Direction = -1f;
        public float speed = 3f;
        private Transform player;

        private void Awake()
        {
            player = FindObjectOfType<CharacterController2D>().transform;
        }

        private void FixedUpdate()
        {
            transform.position += Vector3.left * (speed * Time.deltaTime);
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("ground"))
            {
                speed *= Direction;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                player.transform.parent = transform;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.gameObject.CompareTag("Player"))
            {
                player.transform.parent = null;
            }
        }
    }
}