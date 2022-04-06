using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private float timeAttack = 1f;
    private float maxTimeAttack;
    private PlayerHealth playerHealth;
    private bool isHurts;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
        maxTimeAttack = timeAttack;
    }

    private void Update()
    {
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