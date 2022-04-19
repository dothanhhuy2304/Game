using Game.Enemy;
using UnityEngine;

public class Chicken : EnemyController
{
    [SerializeField] private Collider2D chickenCol;
    [SerializeField] private GameObject explosionObj;
    private SpriteRenderer spriteRenderer;
    private bool isMoving;
    private bool destroy;
    private Vector2 startPos = Vector2.zero;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentTime = maxTimeAttack;
        startPos = transform.position;
    }

    private void OnEnable()
    {
        chickenCol.enabled = true;
    }

    private void Update()
    {
        if (playerHealth.PlayerIsDeath())
        {
            if (!spriteRenderer.enabled)
            {
                StartCoroutine(nameof(Respawns), 3f);
            }
        }

        if (!isMoving) return;
        SetTimeAttack(ref currentTime);
    }

    private void FixedUpdate()
    {
        if (destroy) return;
        if (!isMoving)
        {
            if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            isMoving = true;
        }
        else
        {
            if (Vector3.Distance(transform.position, player.position) > 0.3f)
            {
                Flip();
            }

            MovingToTarget(animationState.chickenIsAttack, true);
            if (currentTime != 0f) return;
            body.bodyType = RigidbodyType2D.Static;
            destroy = true;
            isMoving = false;
            spriteRenderer.enabled = false;
            chickenCol.enabled = false;
            animator.enabled = false;
            explosionObj.transform.position = transform.position;
            explosionObj.SetActive(true);
        }
    }

    private System.Collections.IEnumerator Respawns(float delay)
    {
        yield return new WaitForSeconds(delay);
        isMoving = false;
        body.bodyType = RigidbodyType2D.Kinematic;
        destroy = false;
        spriteRenderer.enabled = true;
        chickenCol.enabled = true;
        animator.enabled = true;
        transform.position = startPos;
        MovingToTarget(animationState.chickenIsAttack, false);
        currentTime = maxTimeAttack;
    }
}