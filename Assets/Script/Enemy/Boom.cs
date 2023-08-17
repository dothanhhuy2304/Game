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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        boomObj.SetActive(false);
                        explosionObj.SetActive(true);
                        colObj.enabled = false;
                        AudioManager.instance.Play("Boom_Explosion");
                        other.collider.GetComponent<PlayerHealth>().RpcGetDamage(30f);
                    }).AppendInterval(1)
                    .AppendCallback(() =>
                    {
                        explosionObj.SetActive(false);
                    }).Play();
            }
        }

    }
}