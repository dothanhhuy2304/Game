using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WallFalling : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D colliders;
    [Range(0, 10)] [SerializeField] private int timeFalling;
    private Vector2 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (HuyManager.PlayerIsDeath())
        {
            WaitToReset();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            WaitToFalling(timeFalling * 1000);
        }
    }

    private async void WaitToFalling(int delay)
    {
        await Task.Delay(delay);
        body.bodyType = RigidbodyType2D.Dynamic;
        colliders.isTrigger = true;
    }

    private async void WaitToReset()
    {
        await Task.Delay(3500);
        body.bodyType = RigidbodyType2D.Static;
        body.transform.position = startPos;
        colliders.isTrigger = false;
    }
}