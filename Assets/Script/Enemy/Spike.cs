using UnityEngine;
using Game.Core;

public class Spike : BaseObject
{
    [SerializeField] private float timeAttack = 1f;
    [SerializeField] private float maxTimeAttack = 1f;
    private PlayerHealth playerHealth;
    private bool isHurts;

    protected override void Start()
    {
        timeAttack = 0f;
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    protected override void Update()
    {
        if (CheckDistance(transform.position, playerHealth.transform.position) > 10f) return;
        if (playerHealth.PlayerIsDeath()) return;
        if (!isHurts) return;
        if (SetTimeAttack(ref timeAttack) != 0f) return;
        playerHealth.GetDamage(20f);
        timeAttack = maxTimeAttack;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isHurts = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isHurts = false;
    }
}