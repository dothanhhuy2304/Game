using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    private Animator animator;
    private PlayerHealth playerHealth;
    private bool isOut;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isOut = false;
        animator.SetBool("hit", true);
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
            animator.SetBool("on", true);
            playerHealth.GetDamage(1f);
            StartCoroutine(nameof(WaitingForFireOn), 0.5f);
        }
        else
        {
            animator.SetBool("hit", false);
            animator.SetBool("on", false);
            yield return null;
        }
    }
}
