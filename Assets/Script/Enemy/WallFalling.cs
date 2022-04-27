using System.Collections;
using UnityEngine;
using Game.Player;

[RequireComponent(typeof(Rigidbody2D))]
public class WallFalling : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    private Collider2D colliders;
    [Range(0f, 10f)] [SerializeField] private float timeFalling;
    private PlayerHealth playerHealth;
    private Vector2 startPos;

    private void Awake()
    {
        startPos = transform.position;
        colliders = GetComponent<Collider2D>();
    }

    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>()?.GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        if (!playerHealth.PlayerIsDeath()) return;
        StartCoroutine(nameof(WaitingReset));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            StartCoroutine(nameof(WaitingFallingDown), timeFalling);
        }
    }

    private IEnumerator WaitingFallingDown(int delay)
    {
        yield return new WaitForSeconds(delay);
        body.bodyType = RigidbodyType2D.Dynamic;
        colliders.isTrigger = true;
        yield return null;
    }

    private IEnumerator WaitingReset()
    {
        yield return new WaitForSeconds(3.5f);
        body.bodyType = RigidbodyType2D.Static;
        body.transform.position = startPos;
        colliders.isTrigger = false;
        yield return null;
    }
}