using UnityEngine;
using Game.Player;

//Improver
namespace Game.Enemy
{
    public class WallMovement : MoveLandController
    {
        private CharacterController2D character;
        [SerializeField] private Vector2 endPos = Vector2.zero;

        private void Start()
        {
            character = CharacterController2D.instance;
            numberLoop = int.MaxValue;
            MoveLandNormal(transform, endPos, timeEndAction, numberLoop);
        }

        // private void Update()
        // {
        //     transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        // }

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