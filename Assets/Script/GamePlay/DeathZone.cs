using Photon.Pun;
using UnityEngine;
using Script.Player;

namespace Script.GamePlay
{
    public class DeathZone : MonoBehaviourPunCallbacks
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerHealth>().RpcDieByFalling();
            }
        }
    }
}