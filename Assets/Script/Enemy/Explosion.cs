using System;
using DG.Tweening;
using Game.GamePlay;
using Game.Player;
using UnityEngine;
using UnityEngine.Events;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private GameObject parentObject;
    public UnityEvent eventTriggerEnter;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        eventTriggerEnter?.Invoke();
    }

    public void EventExplosion()
    {
        StartExplosion();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth.instance.GetDamage(20f);
            col.enabled = false;
        }
    }

    private void StartExplosion()
    {
        DOTween.Sequence()
            .AppendCallback(() =>
            {
                AudioManager.instance.Play("Boom_Explosion");
                HuyManager.CameraShake(cam, 1f, new Vector3(0.5f, 0.5f, 0.5f), 10, 90f, true);
            }).AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                col.enabled = false;
            }).AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                parentObject.SetActive(false);
                col.enabled = true;
                gameObject.SetActive(false);
            }).Play();
    }
}