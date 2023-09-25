using Photon.Pun;
using UnityEngine;
using Script.Player;
using Script.Core;

namespace Script.Enemy
{
    public class Spike : MonoBehaviourPun
    {
        private float _timeAttack;
        [SerializeField] private float maxTimeAttack = 1f;
        private bool _takeDamage;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth.isDeath || playerHealth.isHurt)
                {
                    return;
                }

                HuyManager.Instance.SetUpTime(ref _timeAttack);
                if (_takeDamage)
                {
                    if (_timeAttack <= 0f)
                    {
                        playerHealth.GetDamage(20f);
                        _timeAttack = maxTimeAttack;
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _takeDamage = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _takeDamage = false;
            }
        }
    }
}