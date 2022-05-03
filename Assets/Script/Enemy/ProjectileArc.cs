using Game.Core;
using Game.GamePlay;
using Game.Player;
using UnityEngine;

public class ProjectileArc : BaseObject
{
    private Transform playerPos;
    [SerializeField] private float speed = 10;
    [SerializeField] private float arcHeight = 1;
    private Vector3 startPos = Vector3.zero;
    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject explosionPrefab;
    private Vector3 targetPos = Vector3.zero;

    private void Awake()
    {
        playerPos = FindObjectOfType<CharacterController2D>().transform;
    }

    private void OnEnable()
    {
        startPos = transform.position;
        targetPos = playerPos.position;
    }

    private void Update()
    {
        var x0 = startPos.x;
        var x1 = targetPos.x;
        var dist = x1 - x0;
        var nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
        var baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
        var arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
        var nextPos = new Vector3(nextX, baseY + arc, transform.position.z);
        // Rotate to face the next position, and then move there
        transform.rotation = LookAt2D(nextPos - transform.position);
        transform.position = nextPos;
        if (nextPos == targetPos)
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
            PlayerAudio.Instance.Play("Enemy_Bullet_Explosion_1");
            Explosion();
        }
        else if (other.CompareTag("ground"))
        {
            Arrived();
            PlayerAudio.Instance.Play("Enemy_Bullet_Explosion_1");
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