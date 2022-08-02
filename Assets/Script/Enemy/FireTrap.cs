using System.Collections;
using UnityEngine;
using Game.Player;

public class FireTrap : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private PlayerHealth playerHealth;
    private CharacterController2D player;
    private bool isOut;

    private void Start()
    {
        player = CharacterController2D.instance;
        playerHealth = PlayerHealth.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isOut = true;
            animator.SetBool("hit", true);
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
        if (player.isHurt) yield break;
        if (!isOut) yield break;
        yield return new WaitForSeconds(delay);
        if (isOut)
        {
            animator.SetBool("on", true);
            playerHealth.GetDamage(1f);
            StartCoroutine(nameof(WaitingForFireOn), 0.5f);
        }
        else
        {
            animator.SetBool("hit", false);
            animator.SetBool("on", false);
            StopCoroutine(nameof(WaitingForFireOn));
        }
    }
}
