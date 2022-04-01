using UnityEngine;
using Game.Core;
using Game.Player;

public class PetAI : BaseObject
{
    public Data petData;
    private Transform playerPos;
    private Vector2 velocity = Vector2.zero;
    [SerializeField] private GameObject[] bulletHolder;
    public GameObject[] multipleEnemy;
    [HideInInspector] public Transform closestEnemy;
    private bool enemyContact;
    private float currentTimeAttack;
    private const float TimeAttack = 3f;
    private FireProjectile fireProjectile;
    private PlayerHealth playerHealth;
    private Animator animator;
    private readonly AnimationStates animationState = new AnimationStates();

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        playerPos = FindObjectOfType<CharacterController2D>().transform;
        closestEnemy = null;
        enemyContact = false;
        multipleEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        fireProjectile = bulletHolder[FindBullet()].GetComponent<FireProjectile>();
        playerHealth = playerPos.GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        if (!playerHealth.PlayerIsDeath())
        {
            closestEnemy = FindClosestEnemy();
            if (currentTimeAttack != 0)
            {
                currentTimeAttack -= Time.deltaTime;
            }

            if (Vector3.Distance(transform.position, playerPos.transform.position) < 3f)
            {
                body.velocity = Vector2.zero;
                if (!enemyContact) return;
                if (!(currentTimeAttack <= 0f)) return;
                currentTimeAttack = TimeAttack;
                Attack();
                animator.SetBool(animationState.petIsRun, false);
                //animator.SetBool(IsDeath,false);
            }
            else
            {
                Moving();
                animator.SetBool(animationState.petIsRun, true);
                //animator.SetBool(IsDeath,false);
            }
        }
        else
        {
            body.velocity = Vector2.zero;
            //animator.SetBool(IsDeath, true);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.isTrigger || !other.CompareTag("Enemy")) return;
        closestEnemy.GetComponent<SpriteRenderer>().material.color = new Color(1f, .7f, 0f, 1f);
        enemyContact = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        enemyContact = false;
        other.GetComponent<SpriteRenderer>().material.color = Color.white;
    }

    private void Moving()
    {
        var angle = (playerPos.transform.position - transform.position).normalized;
        body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref velocity, .05f);
    }

    private void Attack()
    {
        bulletHolder[FindBullet()].transform.position = transform.position;
        bulletHolder[FindBullet()].transform.rotation = transform.rotation;
        fireProjectile.SetActives();
    }

    private int FindBullet()
    {
        for (var i = 0; i < bulletHolder.Length; i++)
        {
            if (!bulletHolder[i].activeInHierarchy)
            {
                return i;
            }
        }

        return 0;
    }

    private Transform FindClosestEnemy()
    {
        var closestDistance = Mathf.Infinity;
        Transform trans = null;
        foreach (var go in multipleEnemy)
        {
            if (!go) continue;
            var currentDistance = Vector3.Distance(transform.position, go.transform.position);
            if (!(currentDistance < closestDistance)) continue;
            closestDistance = currentDistance;
            trans = go.transform;
        }

        return trans;
    }
}
