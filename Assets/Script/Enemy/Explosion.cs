using Game.GamePlay;
using UnityEngine;
using Game.Player;

public class Explosion : MonoBehaviour
{
    private bool isAttack = true;

    private void OnEnable()
    {
        isAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerAudio.Instance.Play("Boom_Explosion");
        //playerAudio.Plays_20("Boom_Explosion");
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