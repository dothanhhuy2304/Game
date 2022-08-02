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
        if (HuyManager.PlayerIsDeath())
        {
            if (!boomObj.activeSelf)
            {
                StartCoroutine(nameof(RespawnObject), timeRespawn);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!HuyManager.PlayerIsDeath())
        {
            if (other.collider.CompareTag("Player"))
            {
                PlayerHealth.instance.GetDamage(30f);
                StartCoroutine(Explosion(1f));
            }
        }
    }

    private IEnumerator RespawnObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        boomObj.SetActive(true);
        explosionObj.SetActive(false);
        colObj.enabled = true;
    }

    private IEnumerator Explosion(float delay)
    {
        boomObj.SetActive(false);
        explosionObj.SetActive(true);
        colObj.enabled = false;
        AudioManager.instance.Play("Boom_Explosion");
        yield return new WaitForSeconds(delay);
        explosionObj.SetActive(false);
    }
}