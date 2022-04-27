using System;
using System.Collections;
using UnityEngine;
using Game.Core;
using Game.Player;

public class FireTrap : BaseObject
{
    private Animator animator;
    private PlayerHealth playerHealth;
    private CharacterController2D player;
    private bool isOut;
    private readonly AnimationStates animationState = new AnimationStates();

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<CharacterController2D>().GetComponent<CharacterController2D>();
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isOut = true;
        animator.SetBool(animationState.fireTrapHit, true);
        StartCoroutine(nameof(WaitingForFireOn), 1f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isOut = false;
    }


    private IEnumerator WaitingForFireOn(float delay)
    {
        if (playerHealth.PlayerIsDeath()) yield break;
        if (player.isHurt) yield break;
        if (!isOut) yield break;
        yield return new WaitForSeconds(delay);
        if (isOut)
        {
            animator.SetBool(animationState.fireTrapON, true);
            playerHealth.GetDamage(1f);
            StartCoroutine(nameof(WaitingForFireOn), 0.5f);
        }
        else
        {
            animator.SetBool(animationState.fireTrapHit, false);
            animator.SetBool(animationState.fireTrapON, false);
            StopCoroutine(nameof(WaitingForFireOn));
            yield return null;
        }
    }
}
