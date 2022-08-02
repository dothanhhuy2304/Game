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
        if (!HuyManager.PlayerIsDeath())
        {
            BaseObject.SetTimeAttack(ref timeAttack);
            if (isHurts)
            {
                if (timeAttack <= 0f)
                {
                    playerHealth.GetDamage(20f);
                    timeAttack = maxTimeAttack;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isHurts = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isHurts = false;
        }
    }
}