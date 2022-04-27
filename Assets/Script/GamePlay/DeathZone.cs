using UnityEngine;
using Game.Player;

namespace Game.GamePlay
{
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().Die();
            }
        }
    }
}