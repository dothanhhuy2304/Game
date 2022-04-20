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
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
        timeAttack = 0f;
    }

    private void Update()
    {
        if (!isVisible) return;
        SetTimeAttack(ref timeAttack);
        if (playerHealth.PlayerIsDeath()) return;
        if (!isHurts) return;
        if (timeAttack != 0f) return;
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