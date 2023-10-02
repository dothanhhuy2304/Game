using DG.Tweening;
using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.Enemy
{
    public class Boom : MonoBehaviourPun
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
                        other.collider.GetComponent<PlayerHealth>().GetDamage(30f);
                        HuyManager.Instance.CameraShake(Camera.main, 1f, new Vector3(3f, 3f, 3f), 10, 90f, true);
                    }).AppendInterval(1f)
                    .AppendCallback(() =>
                    {
                        explosionObj.SetActive(false);
                    }).Play();
            }
        }

    }
}