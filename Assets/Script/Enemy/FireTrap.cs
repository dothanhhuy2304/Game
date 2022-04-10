using System.Collections;
using Game.Core;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    private Animator animator;
    private PlayerHealth playerHealth;
    private bool isOut;
    private readonly AnimationStates animationState = new AnimationStates();

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isOut = false;
        animator.SetBool(animationState.fireTrapHit, true);
        StartCoroutine(nameof(WaitingForFireOn), 1f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isOut = true;
    }

    private IEnumerator WaitingForFireOn(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isOut)
        {
            animator.SetBool(animationState.fireTrapON, true);
            playerHealth.GetDamage(1f);
            StartCoroutine(nameof(WaitingForFireOn), 0.5f);
        }
        else
        {
            animator.SetBool(animationState.fireTrapHit, false);
            animator.SetBool(animationState.fireTrapON, false);
            yield return null;
        }
    }
}
