using DG.Tweening;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class WallFalling : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D col;
        [Range(0, 10)] [SerializeField] private int timeFalling;
        private Vector2 startPos;

        private void Awake()
        {
            startPos = transform.position;
            HuyManager.eventResetWhenPlayerDeath += WaitToReset;
        }

        private void WaitToReset()
        {
            DOTween.Sequence()
                .AppendInterval(3.5f)
                .AppendCallback(() =>
                {
                    body.bodyType = RigidbodyType2D.Static;
                    body.transform.position = startPos;
                    col.isTrigger = false;
                }).Play();
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
                    col.isTrigger = true;
                }).Play();
        }
    }
}