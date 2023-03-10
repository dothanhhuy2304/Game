using Game.Player;
using UnityEngine;

public class SpikeMoving : MoveLandController
{
    [SerializeField] private Animator animator;
    [SerializeField] private Vector2 endPos = Vector2.zero;
    private float timeAttack;
    [SerializeField] private float resetTimeAttack = 3f;

    private void Start()
    {
        numberLoop = int.MaxValue;
        MoveLandNormalWithAnimation(transform, endPos, timeEndAction, numberLoop, animator, "TopHit", "ButtomHit");
    }

    // private void Update()
    // {
    //     transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
    // }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!HuyManager.PlayerIsDeath())
        {
            if (other.CompareTag("Player"))
            {
                HuyManager.SetTimeAttack(ref timeAttack);
                if (timeAttack <= 0f)
                {
                    PlayerHealth.instance.GetDamage(20f);
                    timeAttack = resetTimeAttack;
                }
            }
        }
    }
}