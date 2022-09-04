using System;
using Game.GamePlay;
using UnityEngine;
using Game.Player;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject parentObject;
    public UnityEvent eventTriggerEnter;
    private Coroutine currentCoroutine;

    private void OnEnable()
    {
        eventTriggerEnter?.Invoke();
    }

    public void EventExplosion()
    {
        currentCoroutine = StartCoroutine(WaitingHide());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth.instance.GetDamage(20f);
            col.enabled = false;
        }
    }

    private System.Collections.IEnumerator WaitingHide()
    {
        AudioManager.instance.Play("Boom_Explosion");
        yield return new WaitForSeconds(0.2f);
        col.enabled = false;
        yield return new WaitForSeconds(0.5f);
        parentObject.SetActive(false);
        col.enabled = true;
        gameObject.SetActive(false);
        StopCoroutine(currentCoroutine);
    }
}