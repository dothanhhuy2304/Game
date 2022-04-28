using System;
using Game.Core;
using Game.Player;
using UnityEngine;

public class ProjectileArc : BaseObject
{
    private Transform playerPos;
    [SerializeField] private float speed = 10;
    [SerializeField] private float arcHeight = 1;
    private Vector3 startPos = Vector3.zero;

    private Vector3 nextPos = Vector3.zero;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject explosionPrefab;
    private PlayerAudio playerAudio;
    private Vector3 targetPos = Vector3.zero;
    private bool isTringer;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
        playerPos = FindObjectOfType<CharacterController2D>().transform;
        startPos = transform.position;
    }

    private void OnEnable()
    {
        targetPos = playerPos.position;
        isTringer = false;
    }

    private void Update()
    {
        if (!isTringer)
        {
            var x0 = startPos.x;
            var x1 = targetPos.x;
            var dist = x1 - x0;
            var nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
            var baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
            var arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            nextPos = new Vector3(nextX, baseY + arc, transform.position.z);
            // Rotate to face the next position, and then move there
            transform.rotation = LookAt2D(nextPos - transform.position);
            transform.position = nextPos;
        }

        if (nextPos == targetPos || isTringer)
        {
            Arrived();
        }
    }

    private void Arrived()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        other.GetComponent<PlayerHealth>().GetDamage(20f);
        isTringer = true;
        playerAudio.Plays_20("Enemy_Bullet_Explosion_1");
        Arrived();
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