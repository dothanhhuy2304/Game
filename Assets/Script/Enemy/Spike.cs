using UnityEngine;
using Game.Core;
using Game.Player;

public class Spike : MonoBehaviour
{
    private float timeAttack;
    [SerializeField] private float maxTimeAttack = 1f;
    private PlayerHealth playerHealth;
    private bool isHurts;

    private void Start()
    {
        playerHealth = PlayerHealth.instance;
    }

    private void Update()
    {
        BaseObject.SetTimeAttack(ref timeAttack);
        if (HuyManager.PlayerIsDeath()) return;
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