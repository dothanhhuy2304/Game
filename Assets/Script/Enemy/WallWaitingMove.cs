using Script.Player;
using UnityEngine;

namespace Script.Enemy
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

        private SpriteRenderer spriteRenderer;

        private void Start()
        {
            character = FindObjectOfType<CharacterController2D>();
            startTrans = transform.position;
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (spriteRenderer.isVisible)
            {
                if (isComeback)
                {
                    transform.position = Vector3.Lerp(transform.position, startTrans, movingSpeed * Time.deltaTime);
                }
            }

            //
            if (!spriteRenderer.isVisible)
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

            if (other.collider.CompareTag("Player"))
            {
                character.transform.SetParent(transform);
                isMoving = true;
                isComeback = false;
                playerExit = false;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                character.transform.SetParent(null);
                playerExit = true;
            }
        }
    }
}