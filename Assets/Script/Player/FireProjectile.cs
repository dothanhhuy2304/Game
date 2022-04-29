using System;
using System.Collections;
using UnityEngine;
using Game.Core;
using Game.Enemy;
using Game.Player;
using Game.GamePlay;

//Bug
public class FireProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab, explosionPrefab;
    private Transform player;
    private Vector2 target = Vector2.zero;
    private Vector2 targetPetEnemy = Vector2.zero;
    private PlayerHealth playerHealth;
    private PetAI petAI;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController2D>().transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        petAI = FindObjectOfType<PetAI>()?.GetComponent<PetAI>();
    }

    private void OnEnable()
    {
        if (petAI)
        {
            targetPetEnemy = (petAI.closestEnemy.position - transform.position).normalized;
        }

        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
        if (!body.isKinematic) return;
        switch (enemyType)
        {
            case EnemyType.SNINJA:
            {
                body.velocity = SetAngleSNinja() * bulletSpeed;
                break;
            }

            case EnemyType.CarnivorousPlant:
            {
                body.velocity = transform.right * bulletSpeed;
                break;
            }
            case EnemyType.Player:
            {
                body.velocity = transform.right * bulletSpeed;
                break;
            }
            case EnemyType.Pet:
            {
                body.velocity = targetPetEnemy * bulletSpeed;
                break;
            }
            case EnemyType.Bee:
                body.velocity = SetAngleSNinja() * bulletSpeed;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (enemyType)
        {
            case EnemyType.SNINJA:
                if (other.CompareTag("ground"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(20f);
                    EnemyExplosions();
                }

                break;
            case EnemyType.CarnivorousPlant:
                if (other.CompareTag("ground"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(14f);
                    EnemyExplosions();
                }

                break;
            case EnemyType.Bee:
                if (other.CompareTag("ground"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(18f);
                    EnemyExplosions();
                }

                break;
            case EnemyType.Player:
                if (other.gameObject.CompareTag("ground"))
                {
                    PlayerExplosions();
                }
                else if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(playerHealth.playerData.damageAttack);
                    PlayerExplosions();
                }
                else if (other.CompareTag("Box"))
                {
                    Destroy(other.gameObject);
                }

                break;
            case EnemyType.Pet:
                if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(petAI.petData.damageAttack);
                    PlayerExplosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    PlayerExplosions();
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!other.CompareTag("Bullet")) return;
        BulletExplosions();
        PlayerAudio.Instance.Play("Player_Bullet_Explosion_1");
        //playerAudio.Plays_20("Player_Bullet_Explosion_1");

    }

    private Vector2 SetAngleSNinja()
    {
        target = (player.position - transform.position).normalized;
        return target;
    }

    private void PlayerExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        PlayerAudio.Instance.Play("Player_Bullet_Explosion_1");
        //playerAudio.Plays_20("Player_Bullet_Explosion_1");
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
    }

    private void EnemyExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        PlayerAudio.Instance.Play("Player_Bullet_Explosion_1");
        //playerAudio.Plays_20("Enemy_Bullet_Explosion_1");
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
    }

    private void BulletExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
    }

    private IEnumerator TemporarilyDeactivate(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        body.bodyType = RigidbodyType2D.Kinematic;
    }

    public void SetActives()
    {
        gameObject.SetActive(true);
        bulletPrefab.SetActive(true);
        explosionPrefab.SetActive(false);
    }
}