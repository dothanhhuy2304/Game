using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class ObjectMoving : MoveLandController
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Vector2 endPos = Vector2.zero;
        private float timeAttack;
        [SerializeField] private float resetTimeAttack;

        private void Start()
        {
            numberLoop = int.MaxValue;
            MoveLandNormal(transform, endPos, timeEndAction, numberLoop);
            //MoveLandNormalWithAnimation(transform, endPos, timeEndAction, numberLoop, animator, "TopHit", "ButtomHit");
        }

        // private void Update()
        // {
        //     transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        // }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!HuyManager.Instance.PlayerIsDeath())
            {
                if (other.CompareTag("Player"))
                {
                    HuyManager.Instance.SetUpTime(ref timeAttack);
                    if (timeAttack <= 0f)
                    {
                        PlayerHealth.instance.GetDamage(20f);
                        timeAttack = resetTimeAttack;
                    }
                }
            }
        }
    }
}