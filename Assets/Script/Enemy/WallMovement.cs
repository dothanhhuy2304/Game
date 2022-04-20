using UnityEngine;
using Game.Core;
using Game.Player;

//Improver
namespace Game.Enemy
{
    public class WallMovement : BaseObject
    {
        public float speed = 3f;
        private Transform player;
        private Vector2 startPos = Vector2.zero;
        [SerializeField] private Vector2 endPos = Vector2.zero;
        [SerializeField] private float timeSleep;

        protected override void Start()
        {
            player = FindObjectOfType<CharacterController2D>()?.transform;
            startPos = transform.position;
        }

        private void Update()
        {
            if (!isVisible) return;
            transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = transform;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = null;
            }
        }
    }
}