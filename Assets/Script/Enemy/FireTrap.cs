using System.Collections;
using Game.Player;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private PlayerHealth playerHealth;
    private bool isFirst;
    private Coroutine currentCoroutine;

    private void Start()
    {
        playerHealth = PlayerHealth.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            currentCoroutine = StartCoroutine(IeFireOn(1f));
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

    private IEnumerator IeFireOn(float delay)
    {
        if (HuyManager.PlayerIsDeath()) yield break;
        if (HuyManager.GetPlayerIsHurt()) yield break;
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
            yield return new WaitForSeconds(delay);
        }

        playerHealth.GetDamage(1f);
        isFirst = false;
        currentCoroutine = StartCoroutine(IeFireOn(delay));
    }
}
