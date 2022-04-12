using System;
using System.Collections;
using UnityEngine;
using Game.Core;
using Game.Enemy;
using Game.Player;

//Bug
public class FireProjectile : MonoBehaviour
{
    [SerializeField]private Rigidbody2D body;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab, explosionPrefab;
    private Transform player;
    private Vector2 target = Vector2.zero;
    private Vector2 targetPetEnemy = Vector2.zero;
    private PlayerHealth playerHealth;
    private PetAI petAI;
    private PlayerAudio playerAudio;

    private void Awake()
    {
        if (!body)
        {
            body = GetComponent<Rigidbody2D>();
        }

        player = FindObjectOfType<CharacterController2D>().transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        petAI = FindObjectOfType<PetAI>().GetComponent<PetAI>();
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
    }

    private void OnEnable()
    {
        targetPetEnemy = (petAI.closestEnemy.position - transform.position).normalized;
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
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (enemyType)
        {
            case EnemyType.SNINJA:
            {
                if (other.CompareTag("ground"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(20f);
                    EnemyExplosions();
                }

                break;
            }
            case EnemyType.CarnivorousPlant:
            {
                if (other.CompareTag("ground"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    EnemyExplosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(14f);
                    EnemyExplosions();
                }

                break;
            }
            case EnemyType.Player:
            {
                if (other.gameObject.CompareTag("ground"))
                {
                    PlayerExplosions();
                }
                else if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(playerHealth.playerData.damageAttack);
                    PlayerExplosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    PlayerExplosions();
                }
                else if (other.CompareTag("Box"))
                {
                    Destroy(other.gameObject);
                }

                break;
            }
            case EnemyType.Pet:
            {
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
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        //AudioSource.PlayClipAtPoint(soundExplosion[0], transform.position, 1f);
        playerAudio.Plays_20("Player_Bullet_Explosion_1");
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
    }

    private void EnemyExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        //AudioSource.PlayClipAtPoint(soundExplosion[0], transform.position, 1f);
        playerAudio.Plays_20("Enemy_Bullet_Explosion_1");
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