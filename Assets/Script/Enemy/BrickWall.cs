using Game.Core;
using UnityEngine;

public class BrickWall : BaseObject
{

    [SerializeField] private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        if (spriteRenderer) return;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.material.color = Color.gray;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spriteRenderer.material.color = Color.white;
        }
    }
}
