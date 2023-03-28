using DG.Tweening;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Boom : MonoBehaviour
    {
        [SerializeField] private GameObject boomObj, explosionObj;
        [SerializeField] private Collider2D colObj;
        [SerializeField] private float timeRespawn;
        private bool isReSpawn;

        private void Awake()
        {
            HuyManager.eventResetWhenPlayerDeath += WaitToRest;
        }

        private void WaitToRest()
        {
            if (HuyManager.PlayerIsDeath() && !isReSpawn)
            {
                ReSpawnObject(timeRespawn);
                isReSpawn = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {

            if (other.collider.CompareTag("Player"))
            {
                PlayExplosion(1);
            }
        }

        private void ReSpawnObject(float delay)
        {
            DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    boomObj.SetActive(true);
                    explosionObj.SetActive(false);
                    colObj.enabled = true;
                }).Play();
        }

        private void PlayExplosion(float delay)
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    boomObj.SetActive(false);
                    explosionObj.SetActive(true);
                    colObj.enabled = false;
                    AudioManager.instance.Play("Boom_Explosion");
                    PlayerHealth.instance.GetDamage(30f);
                }).AppendInterval(delay)
                .AppendCallback(() =>
                {
                    explosionObj.SetActive(false);
                    isReSpawn = false;
                }).Play();
        }
    }
}