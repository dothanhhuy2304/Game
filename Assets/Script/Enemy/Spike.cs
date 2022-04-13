using Game.Core;
using UnityEngine;

public class Spike : BaseObject
{
    [SerializeField] private float timeAttack = 1f;
    private float maxTimeAttack;
    private PlayerHealth playerHealth;
    private bool isHurts;

    protected override void Start()
    {
        maxTimeAttack = timeAttack;
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    protected override void Update()
    {
        if (base.CheckDistance(transform.position, playerHealth.transform.position) > 10f) return;
        if (playerHealth.PlayerIsDeath()) return;
        if (timeAttack != 0f)
        {
            timeAttack -= Time.deltaTime;
        }

        if (timeAttack <= 0f)
        {
            timeAttack = 0f;
        }

        if (!isHurts) return;
        if (timeAttack > 0f) return;
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