using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Script.Core;

public class ITest : MonoBehaviour
{
    [SerializeField] public Sprite[] idleAnimation, collectedAnimation;
    [SerializeField] public Sprite[] sprites = new Sprite[0];
    public SpriteRenderer spriteRenderer;
    public float index = -1;
    public FrameController controller;
    [SerializeField] private bool randomAnimationStartTime;
    public int frame;
    public bool collected;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (randomAnimationStartTime)
            frame = Random.Range(0, sprites.Length);
        sprites = idleAnimation;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnter();
        }
    }


    private void OnPlayerEnter()
    {
        if (collected) return;
        frame = 0;
        sprites = collectedAnimation;
    }
}
