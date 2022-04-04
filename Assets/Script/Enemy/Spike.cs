using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        StartCoroutine(nameof(Hurt), 1f);
    }

    private IEnumerator Hurt(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerHealth.GetDamage(20f);
    }
}
