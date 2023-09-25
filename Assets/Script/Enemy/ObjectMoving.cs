using DG.Tweening;
using Photon.Pun;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class ObjectMoving : MonoBehaviourPun
    {
        [SerializeField] protected float timeEndAction;
        [SerializeField] private Vector2 endPos = Vector2.zero;
        private float _timeAttack;
        [SerializeField] private float resetTimeAttack;

        private void Start()
        {
            MoveLandNormal(transform, endPos, timeEndAction, int.MaxValue);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (!playerHealth.isDeath)
                {
                    HuyManager.Instance.SetUpTime(ref _timeAttack);
                    if (_timeAttack <= 0f)
                    {
                        playerHealth.GetDamage(20f);
                        _timeAttack = resetTimeAttack;
                    }
                }
            }
        }
        
        
        protected static void MoveLandNormal(Transform startPosition, Vector2 endPosition, float timeEndActions, int loop)
        {
            startPosition.DOMove(endPosition, timeEndActions).SetEase(Ease.Linear).SetLoops(loop, LoopType.Yoyo);
        }
    }
}