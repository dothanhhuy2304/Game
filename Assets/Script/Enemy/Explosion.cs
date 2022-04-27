using UnityEngine;
using Game.Player;

public class Explosion : MonoBehaviour
{
    private bool isAttack = true;
    private PlayerAudio playerAudio;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
    }

    private void OnEnable()
    {
        isAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerAudio.Plays_20("Boom_Explosion");
        StartCoroutine(nameof(WaitingHide), 0.7f);
        if (!isAttack) return;
        if (!other.CompareTag("Player")) return;
        isAttack = false;
        other.GetComponent<PlayerHealth>().GetDamage(20f);
    }

    private System.Collections.IEnumerator WaitingHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}