using System.Collections;
using Script.Player;
using UnityEngine;

namespace Script.Enemy
{
    public class FireTrap : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private bool _isFirst;
        private Coroutine _currentCoroutine;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _currentCoroutine = StartCoroutine(IeFireOn(other.GetComponent<PlayerHealth>()));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                animator.Play("Idle");
                _isFirst = true;
                StopCoroutine(_currentCoroutine);
            }
        }

        private IEnumerator IeFireOn(PlayerHealth playerHealth)
        {
            if (playerHealth.isDeath || playerHealth.isHurt) yield break;
            if (_isFirst)
            {
                animator.Play("Begin");
                yield return new WaitForSeconds(0.6f);
            }

            animator.Play("On");
            if (_isFirst)
            {
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                yield return new WaitForSeconds(1f);
            }

            playerHealth.GetDamage(1f);
            _isFirst = false;
            _currentCoroutine = StartCoroutine(IeFireOn(playerHealth));
        }
    }
}