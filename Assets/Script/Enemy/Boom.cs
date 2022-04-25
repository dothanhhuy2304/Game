using System.Collections;
using Game.Core;
using UnityEngine;

public class Boom : BaseObject
{
    private Collider2D col;
    [SerializeField] private GameObject boomObj, explosionObj;
    [SerializeField] private Collider2D colObj;
    private PlayerHealth playerHealth;
    private PlayerAudio playerAudio;
    [SerializeField] private float timeRespawn;

    protected override void Start()
    {
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
        colObj = GetComponent<Collider2D>();
        playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!isVisible)
        {
            col.enabled = false;
        }
        else
        {
            col.enabled = true;
            if (!playerHealth.PlayerIsDeath()) return;
            if (boomObj.activeSelf) return;
            StartCoroutine(nameof(RespawnObject), timeRespawn);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<PlayerHealth>().PlayerIsDeath()) return;
        if (!other.collider.CompareTag("Player")) return;
        other.collider.GetComponent<PlayerHealth>().GetDamage(30f);
        StartCoroutine(nameof(Explosion), 1f);
    }

    private IEnumerator RespawnObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        boomObj.SetActive(true);
        explosionObj.SetActive(false);
        colObj.enabled = true;
        yield return null;
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
