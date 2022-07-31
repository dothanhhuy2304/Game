using UnityEngine;
using Game.Core;
using Game.Player;

namespace Game.Enemy
{
    public class WallWaitingMove : MonoBehaviour
    {
        private const float Direction = -1f;
        private CharacterController2D character;
        private bool isMoving;
        [SerializeField] private float movingSpeed = 2f;
        private bool isComeback;
        private bool playerExit;
        private Vector3 startTrans = Vector3.zero;
        private bool isVisible;
        private void Start()
        {
            character = CharacterController2D.instance;
            startTrans = transform.position;
        }

        private void Update()
        {
            if (isVisible)
            {
                if (isComeback)
                {
                    transform.position = Vector3.Lerp(transform.position, startTrans, movingSpeed * Time.deltaTime);
                }
            }

            //
            if (!isVisible)
            {
                isComeback = true;
            }

            if (isMoving)
            {
                transform.position += Vector3.right * (movingSpeed * Time.deltaTime);
            }

            if (isComeback)
            {
                transform.position = Vector3.Lerp(transform.position, startTrans, movingSpeed * Time.deltaTime);
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
            character.transform.parent = transform;
            isMoving = true;
            isComeback = false;
            playerExit = false;
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!other.collider.CompareTag("Player")) return;
            character.transform.parent = null;
            playerExit = true;
        }
        
        
        private void OnBecameVisible()
        {
            isVisible = true;
        }

        private void OnBecameInvisible()
        {
            isVisible = false;
        }
    }
}