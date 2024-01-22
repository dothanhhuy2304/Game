using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.Enemy
{
    public class ProjectileArc : MonoBehaviour
    {
        private CharacterController2D _player;
        private Rigidbody2D _body;
        [SerializeField] private float speed = 10;
        [SerializeField] private float arcHeight = 1;
        private Vector3 _startPos = Vector3.zero;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject explosionPrefab;
        private Vector3 _targetPos = Vector3.zero;

        private void Awake()
        {
            _player = CharacterController2D.IsLocalPlayer;
            _body = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _startPos = transform.position;
            _targetPos = _player.transform.position;
        }

        private void Update()
        {
            float x0 = _startPos.x;
            float x1 = _targetPos.x;
            float dist = x1 - x0;
            Vector3 trans = transform.position;
            float nextX = Mathf.MoveTowards(trans.x, x1, speed * Time.deltaTime);
            float baseY = Mathf.Lerp(_startPos.y, _targetPos.y, (nextX - x0) / dist);
            float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, baseY + arc, trans.z);
            // Rotate to face the next position, and then move there
            transform.rotation = LookAt2D(nextPos - trans);
            // ReSharper disable once Unity.InefficientPropertyAccess
            transform.position = nextPos;
            if (nextPos == _targetPos)
            {
                Arrived();
            }
        }

        private void Explosion()
        {
            bulletPrefab.SetActive(false);
            explosionPrefab.SetActive(true);
            StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
        }

        private void Arrived()
        {
            bulletPrefab.SetActive(false);
            StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().GetDamage(20f);
                AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                Explosion();
            }
            else if (other.CompareTag("ground"))
            {
                Arrived();
                AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
            }
            else if (other.CompareTag("Bullet"))
            {
                if (!_body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletEnemy")))
                {
                    Arrived();
                    AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                }
            }
        }

        private System.Collections.IEnumerator TemporarilyDeactivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        public void SetActives()
        {
            gameObject.SetActive(true);
            bulletPrefab.SetActive(true);
            explosionPrefab.SetActive(false);
        }

        private static Quaternion LookAt2D(Vector2 forward)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
        }
    }
}