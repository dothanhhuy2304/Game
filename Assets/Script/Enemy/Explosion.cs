using Game.GamePlay;
using UnityEngine;
using Game.Player;

public class Explosion : MonoBehaviour
{
    //private bool isAttack;
    private PlayerAudio playerAudio;
    [SerializeField] private Collider2D col;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
    }

    private void OnEnable()
    {
        //isAttack = true;
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerAudio.Play("Boom_Explosion");
        StartCoroutine(nameof(WaitingHide), 0.7f);
        //if (!isAttack) return;
        if (!other.CompareTag("Player")) return;
        //isAttack = false;
        other.GetComponent<PlayerHealth>().GetDamage(20f);
    }

    private System.Collections.IEnumerator WaitingHide(float delay)
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}