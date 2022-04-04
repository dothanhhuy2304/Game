using UnityEngine;

public class Car : MonoBehaviour
{
    private Vector2 trans;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        trans = transform.position;
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (!playerHealth.PlayerIsDeath()) return;
        transform.position = trans;
    }
}