using System;
using System.Collections;
using UnityEngine;
using Game.Core;
using Game.Enemy;
using Game.Player;

//Bug
public class FireProjectile : MonoBehaviour
{
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab, explosionPrefab;
    [SerializeField] private AudioClip[] soundExplosion;
    private Transform player;
    private Vector2 target = Vector2.zero;
    private Vector2 targetPetEnemy = Vector2.zero;
    private PlayerHealth playerHealth;
    private PetAI petAI;
    private PlayerAudio playerAudio;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        player = FindObjectOfType<CharacterController2D>().transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        petAI = FindObjectOfType<PetAI>().GetComponent<PetAI>();
        playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
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
                    Explosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    Explosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(20f);
                    Explosions();
                }

                break;
            }
            case EnemyType.CarnivorousPlant:
            {
                if (other.CompareTag("ground"))
                {
                    Explosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    Explosions();
                }
                else if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(14f);
                    Explosions();
                }

                break;
            }
            case EnemyType.Player:
            {
                if (other.gameObject.CompareTag("ground"))
                {
                    Explosions();
                }
                else if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(playerHealth.playerData.damageAttack);
                    Explosions();
                }
                else if (other.CompareTag("Bullet"))
                {
                    Explosions();
                }
                else if (other.CompareTag("Box"))
                {
                    Destroy(other.gameObject);
                }

                break;
            }
            case EnemyType.Pet:
            {
                if (other.gameObject.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(petAI.petData.damageAttack);
                    Explosions();
                }
                else if (other.gameObject.CompareTag("Bullet"))
                {
                    Explosions();
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

    private void Explosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        //AudioSource.PlayClipAtPoint(soundExplosion[0], transform.position, 1f);
        playerAudio.Play(soundExplosion[0]);
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