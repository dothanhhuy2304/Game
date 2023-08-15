using System;
using DG.Tweening;
using Photon.Pun;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Boom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject boomObj, explosionObj;
        [SerializeField] private Collider2D colObj;
        [SerializeField] private float timeRespawn;
        private bool isReSpawn;

        private void Awake()
        {
            HuyManager.Instance.eventResetWhenPlayerDeath += WaitToRest;
        }

        private void WaitToRest()
        {
            if (HuyManager.Instance.PlayerIsDeath() && !isReSpawn)
            {
                DOTween.Sequence()
                    .AppendInterval(timeRespawn)
                    .AppendCallback(() =>
                    {
                        isReSpawn = true;
                        boomObj.SetActive(true);
                        explosionObj.SetActive(false);
                        colObj.enabled = true;
                    });
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                //PlayExplosion(1, other.collider.GetComponent<CharacterController2D>());
                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        boomObj.SetActive(false);
                        explosionObj.SetActive(true);
                        colObj.enabled = false;
                        AudioManager.instance.Play("Boom_Explosion");
                        other.collider.GetComponent<CharacterController2D>().playerHealth.GetDamage(30f);
                    }).AppendInterval(1)
                    .AppendCallback(() =>
                    {
                        explosionObj.SetActive(false);
                        isReSpawn = false;
                    }).Play();
            }
        }

    }
}