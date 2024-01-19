using DG.Tweening;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Explosion : MonoBehaviour
    {

        private void OnEnable()
        {
            StartExplosion();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GetComponent<Collider2D>().enabled = false;
                other.GetComponent<PlayerHealth>().GetDamage(20f);

            }
        }

        private void StartExplosion()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    AudioManager.instance.Play("Boom_Explosion");
                    HuyManager.CameraShake(Camera.main, 0.5f, new Vector3(5f, 5f, 0f), 30, 90f, true);
                }).AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    GetComponent<Collider2D>().enabled = false;
                })
                .AppendInterval(0.5f)
                .AppendCallback(() => { gameObject.SetActive(false); }).Play();
        }
    }
}