using UnityEngine;
using Game.Player;

public class SpikeHead : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField] private Vector2 endPos = Vector2.zero;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float timeSleep;
    private float timeAttack;
    [SerializeField] private float resetTimeAttack = 2f;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        //HuyManager.SetTimeAttack(ref timeAttack);
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HuyManager.SetTimeAttack(ref timeAttack);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!HuyManager.PlayerIsDeath())
            {
                if (timeAttack <= 0)
                {
                    PlayerHealth.instance.GetDamage(20f);
                    timeAttack = resetTimeAttack;
                }
            }
        }
    }
}