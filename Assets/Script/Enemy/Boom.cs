using System.Collections;
using Game.GamePlay;
using UnityEngine;
using Game.Player;

public class Boom : MonoBehaviour
{
    [SerializeField] private GameObject boomObj, explosionObj;
    [SerializeField] private Collider2D colObj;
    [SerializeField] private float timeRespawn;
    
    private void Update()
    {
        if (!HuyManager.PlayerIsDeath()) return;
        if (boomObj.activeSelf) return;
        StartCoroutine(nameof(RespawnObject), timeRespawn);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (HuyManager.PlayerIsDeath()) return;
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
        AudioManager.instance.Play("Boom_Explosion");
        //playerAudio.Plays_20("Boom_Explosion");
        yield return new WaitForSeconds(delay);
        explosionObj.SetActive(false);
        yield return null;
    }
}