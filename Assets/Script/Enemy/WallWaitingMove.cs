using Photon.Pun;
using UnityEngine;

namespace Script.Enemy
{
    public class WallWaitingMove : MonoBehaviourPun
    {
        private const float Direction = -1f;
        private bool _isMoving;
        [SerializeField] private float movingSpeed = 2f;
        private bool _isComeback;
        private bool _playerExist;
        private Vector3 _startTrans = Vector3.zero;

        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _startTrans = transform.position;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_spriteRenderer.isVisible)
            {
                if (_isComeback)
                {
                    transform.position = Vector3.Lerp(transform.position, _startTrans, movingSpeed * Time.deltaTime);
                }
            }

            //
            if (!_spriteRenderer.isVisible)
            {
                _isComeback = true;
            }

            if (_isMoving)
            {
                transform.position += Vector3.right * (movingSpeed * Time.deltaTime);
            }

            if (_isComeback)
            {
                transform.position = Vector3.Lerp(transform.position, _startTrans, movingSpeed * Time.deltaTime);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("ground"))
            {
                if (!_playerExist)
                {
                    movingSpeed *= Direction;
                }
                else
                {
                    movingSpeed *= Direction;
                    _isComeback = true;
                }
            }

            if (other.collider.CompareTag("Player"))
            {
                other.transform.SetParent(transform);
                _isMoving = true;
                _isComeback = false;
                _playerExist = false;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                other.transform.SetParent(null);
                _playerExist = true;
            }
        }
    }
}