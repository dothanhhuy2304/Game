using UnityEngine;
using Script.Player;

namespace Script.GamePlay
{
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                FindObjectOfType<PlayerHealth>().DieByFalling();
            }
        }
    }
}