using System.Collections;
using UnityEngine;

public class Boom : MonoBehaviour
{
    [SerializeField] private GameObject boomObj, explosionObj;
    [SerializeField] private PolygonCollider2D colObj;
    private PlayerAudio playerAudio;

    private void Awake()
    {
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Player")) return;
        boomObj.SetActive(false);
        explosionObj.SetActive(true);
        colObj.enabled = false;
        other.collider.GetComponent<PlayerHealth>().GetDamage(30f);
        playerAudio.Plays_20("Boom_Explosion");
        StartCoroutine(nameof(Explosion), 1f);
    }

    private IEnumerator Explosion(float delay)
    {
        yield return new WaitForSeconds(delay);
        explosionObj.SetActive(false);
    }
}
