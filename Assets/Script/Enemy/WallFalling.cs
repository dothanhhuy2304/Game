using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallFalling : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D colliders;
    [Range(0, 10)] [SerializeField] private int timeFalling;
    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (HuyManager.PlayerIsDeath())
        {
            WaitToReset();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            WaitToFalling(timeFalling);
        }
    }

    private void WaitToFalling(int delay)
    {
        DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() =>
            {
                body.bodyType = RigidbodyType2D.Dynamic;
                colliders.isTrigger = true;
            }).Play();
    }

    private void WaitToReset()
    {
        DOTween.Sequence()
            .AppendInterval(3.5f)
            .AppendCallback(() =>
            {
                body.bodyType = RigidbodyType2D.Static;
                body.transform.position = startPos;
                colliders.isTrigger = false;
            }).Play();
    }
}