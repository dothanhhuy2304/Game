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
    private float currentTimeAttack = 3f;
    private const float TimeAttack = 3f;
    private PlayerHealth playerHealth;
    [SerializeField] private Animator animator;
    private readonly AnimationStates animationState = new AnimationStates();

    protected override void Start()
    {
        base.Start();
        playerPos = FindObjectOfType<CharacterController2D>().transform;
        closestEnemy = null;
        enemyContact = false;
        multipleEnemy = GameObject.FindGameObjectsWithTag("Enemy");
        playerHealth = playerPos.GetComponent<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        if (!playerHealth.PlayerIsDeath())
        {
            SetTimeAttack(ref currentTimeAttack);
            closestEnemy = FindClosestEnemy();
            if (Vector3.Distance(transform.position, playerPos.transform.position) < 3f)
            {
                body.velocity = Vector2.zero;
                if (!enemyContact) return;
                if (currentTimeAttack != 0f) return;
                StartCoroutine(nameof(Attacks));
                currentTimeAttack = TimeAttack;
                animator.SetBool(animationState.petIsRun, false);
            }
            else
            {
                Moving();
                animator.SetBool(animationState.petIsRun, true);
            }
        }
        else
        {
            body.velocity = Vector2.zero;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.isTrigger || !other.CompareTag("Enemy")) return;
        closestEnemy.GetComponent<SpriteRenderer>().color = new Color(1f, .7f, 0f, 1f);
        enemyContact = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        enemyContact = false;
        other.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void Moving()
    {
        var angle = (playerPos.transform.position - transform.position).normalized;
        body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref velocity, .05f);
    }

    private System.Collections.IEnumerator Attacks()
    {
        yield return new WaitForSeconds(0f);
        Attack();
        yield return null;
    }
    

    private void Attack()
    {
        bulletHolder[FindBullet()].transform.position = transform.position;
        bulletHolder[FindBullet()].transform.rotation = transform.rotation;
        bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
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
