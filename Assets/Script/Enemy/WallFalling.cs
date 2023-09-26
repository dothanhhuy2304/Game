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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                WaitToFalling();
            }
        }

        private void WaitToFalling()
        {
            DOTween.Sequence()
                .AppendInterval(timeFalling)
                .AppendCallback(() =>
                {
                    body.bodyType = RigidbodyType2D.Dynamic;
                    col.isTrigger = true;
                }).Play();
        }
    }
}