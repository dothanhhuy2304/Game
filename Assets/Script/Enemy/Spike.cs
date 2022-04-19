using UnityEngine;
using Game.Core;
using Game.Player;

public class Spike : BaseObject
{
    [SerializeField] private float timeAttack = 1f;
    [SerializeField] private float maxTimeAttack = 1f;
    private PlayerHealth playerHealth;
    private bool isHurts;
    private Transform player;

    protected override void Start()
    {
        player = FindObjectOfType<CharacterController2D>().transform;
        timeAttack = 0f;
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        base.CheckDistance(player.position, transform.position);
        SetTimeAttack(ref timeAttack);
        if (!hasInteracted) return;
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