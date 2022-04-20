using UnityEngine;
using Game.Core;

public class SpikeHead : BaseObject
{
    private Vector3 startPos;
    [SerializeField] private Vector2 endPos = Vector2.zero;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float timeSleep;
    [SerializeField] private float timeAttack;
    [SerializeField] private float resetTimeAttack = 2f;

    private void Awake()
    {
        startPos = transform.position;
        timeAttack = 0f;
    }

    private void Update()
    {
        if (!isVisible) return;
        transform.position = Vector2.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
        SetTimeAttack(ref timeAttack);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.GetComponent<PlayerHealth>().PlayerIsDeath()) return;
        if (timeAttack != 0) return;
        other.GetComponent<PlayerHealth>().GetDamage(20f);
        timeAttack = resetTimeAttack;
    }
}