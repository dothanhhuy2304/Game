using System.Collections;
using UnityEngine;
using Game.Player;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private PlayerHealth playerHealth;
    private bool isOut;

    private void Start()
    {
        playerHealth = PlayerHealth.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isOut = true;
            StartCoroutine(WaitingForFireOn(1f));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isOut = false;
        }
    }

    private IEnumerator WaitingForFireOn(float delay)
    {
        if (HuyManager.PlayerIsDeath()) yield break;
        if (HuyManager.GetPlayerIsHurt()) yield break;
        if (!isOut) yield break;
        animator.SetBool("hit", true);
        yield return new WaitForSeconds(delay);
        if (isOut)
        {
            animator.SetBool("on", true);
            playerHealth.GetDamage(1f);
            StartCoroutine(WaitingForFireOn(0.5f));
        }
        else
        {
            animator.SetBool("hit", false);
            animator.SetBool("on", false);
            StopCoroutine(nameof(WaitingForFireOn));
        }
    }
}
