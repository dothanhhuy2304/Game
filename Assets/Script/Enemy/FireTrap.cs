using System.Collections;
using Photon.Pun;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class FireTrap : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Animator animator;
        private bool isFirst;
        private Coroutine currentCoroutine;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                currentCoroutine = StartCoroutine(IeFireOn(other.GetComponent<PlayerHealth>()));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                animator.Play("Idle");
                isFirst = true;
                StopCoroutine(currentCoroutine);
            }
        }

        private IEnumerator IeFireOn(PlayerHealth playerHealth)
        {
            if (playerHealth.isDeath || playerHealth.isHurt) yield break;
            if (isFirst)
            {
                animator.Play("Begin");
                yield return new WaitForSeconds(0.6f);
            }

            animator.Play("On");
            if (isFirst)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }

            playerHealth.RpcGetDamage(1f);
            isFirst = false;
            currentCoroutine = StartCoroutine(IeFireOn(playerHealth));
        }
    }
}