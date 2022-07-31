using UnityEngine;
using Game.Player;

//Improver
namespace Game.Enemy
{
    public class WallMovement : MonoBehaviour
    {
        public float speed = 3f;
        private CharacterController2D character;
        private Vector2 startPos = Vector2.zero;
        [SerializeField] private Vector2 endPos = Vector2.zero;
        [SerializeField] private float timeSleep;

        private void Start()
        {
            character = CharacterController2D.instance;
            startPos = transform.position;
        }

        private void Update()
        {
            transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                character.transform.parent = transform;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                character.transform.parent = null;
            }
        }
    }
}