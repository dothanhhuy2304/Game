using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace Script.Enemy
{
    public class LandMovement : MonoBehaviourPun
    {
        [SerializeField] protected float timeEndAction;
        [SerializeField] private Vector2 endPos = Vector2.zero;

        private void Start()
        {
            MoveLandNormal(transform, endPos, timeEndAction, int.MaxValue);
        }

        // private void Update()
        // {
        //     transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        // }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                other.transform.SetParent(transform);
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }
        }
        
        private static void MoveLandNormal(Transform startPosition, Vector2 endPosition, float timeEndActions, int loop)
        {
            startPosition.DOMove(endPosition, timeEndActions).SetEase(Ease.Linear).SetLoops(loop, LoopType.Yoyo);
        }
    }
}