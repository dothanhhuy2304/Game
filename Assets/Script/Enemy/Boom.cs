using System.Collections;
using UnityEngine;

public class Boom : MonoBehaviour
{
    [SerializeField] private GameObject boomObj, explosionObj;
    [SerializeField] private Collider2D colObj;
    private PlayerAudio playerAudio;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
        colObj = gameObject.GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<PlayerHealth>().PlayerIsDeath()) return;
        if (!other.collider.CompareTag("Player")) return;
        other.collider.GetComponent<PlayerHealth>().GetDamage(30f);
        StartCoroutine(nameof(Explosion), 1f);
    }

    private IEnumerator Explosion(float delay)
    {
        boomObj.SetActive(false);
        explosionObj.SetActive(true);
        colObj.enabled = false;
        playerAudio.Plays_20("Boom_Explosion");
        yield return new WaitForSeconds(delay);
        explosionObj.SetActive(false);
        yield return null;
    }
}
