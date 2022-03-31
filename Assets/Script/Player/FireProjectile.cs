using System;
using System.Collections;
using UnityEngine;
using Game.Core;
using Game.Enemy;
using Game.Player;

//Bug
public class FireProjectile : BaseObject
{
    [SerializeField] private Data[] datas;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab, explosionPrefab;
    [SerializeField] private AudioClip[] soundExplosion;
    private Transform player;
    private Vector3 target = Vector3.zero;
    private Vector3 targetPetEnemy = Vector3.zero;
    private PlayerHealth playerHealth;
    private PetAI petAI;

    public override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<CharacterController2D>().transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        petAI = FindObjectOfType<PetAI>();
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
                body.velocity = SetAngle() * bulletSpeed;
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
                if (other.gameObject.CompareTag("ground"))
                {
                    Explosions();
                }
                else if (other.gameObject.CompareTag("Bullet"))
                {
                    Explosions();
                }
                else if (other.gameObject.CompareTag("Player"))
                {
                    //playerHealth.GetDamage(20f);
                    playerHealth.GetDamage(datas[2].damageAttack);
                    Explosions();
                }

                break;
            }
            case EnemyType.CarnivorousPlant:
            {
                if (other.gameObject.CompareTag("ground"))
                {
                    Explosions();
                }
                else if (other.gameObject.CompareTag("Bullet"))
                {
                    Explosions();
                }
                else if (other.gameObject.CompareTag("Player"))
                {
                    //playerHealth.GetDamage(14f);
                    playerHealth.GetDamage(datas[3].damageAttack);
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
                else if (other.gameObject.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(playerHealth.playerData.damageAttack);
                    Explosions();
                }
                else if (other.gameObject.CompareTag("Bullet"))
                {
                    Explosions();
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

    private Vector3 SetAngle()
    {
        target = (player.position - transform.position).normalized;
        return target;
    }

    private void Explosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        AudioSource.PlayClipAtPoint(soundExplosion[0], transform.position, 1f);
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