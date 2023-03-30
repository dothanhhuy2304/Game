using UnityEngine;
using Script.Player;
using Script.Core;

namespace Script.Enemy
{
    public class Spike : MonoBehaviour
    {
        private float timeAttack;
        [SerializeField] private float maxTimeAttack = 1f;
        private bool isHurts;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (HuyManager.PlayerIsDeath() || HuyManager.GetPlayerIsHurt()) return;
                HuyManager.SetUpTime(ref timeAttack);
                if (isHurts)
                {
                    if (timeAttack <= 0f)
                    {
                        PlayerHealth.instance.GetDamage(20f);
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
}