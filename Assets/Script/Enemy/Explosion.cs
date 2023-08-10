using DG.Tweening;
using Script.Player;
using UnityEngine;
using Script.Core;

namespace Script.Enemy
{
    public class Explosion : MonoBehaviour
    {
        private Camera cam;

        private void Start()
        {
            cam = Camera.main;
        }

        private void OnEnable()
        {
            StartExplosion();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                FindObjectOfType<PlayerHealth>().GetDamage(20f);
                GetComponent<Collider2D>().enabled = false;
            }
        }

        private void StartExplosion()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    AudioManager.instance.Play("Boom_Explosion");
                    HuyManager.Instance.CameraShake(cam, 1f, new Vector3(0.5f, 0.5f, 0.5f), 10, 90f, true);
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