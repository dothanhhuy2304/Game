using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallFalling : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D colliders;
    [Range(0f, 10f)] [SerializeField] private float timeFalling;
    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (HuyManager.PlayerIsDeath())
        {
            StartCoroutine(nameof(WaitingReset));
        }
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
    }

    private IEnumerator WaitingReset()
    {
        yield return new WaitForSeconds(3.5f);
        body.bodyType = RigidbodyType2D.Static;
        body.transform.position = startPos;
        colliders.isTrigger = false;
    }
}