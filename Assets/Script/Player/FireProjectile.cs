using System;
using UnityEngine;
using Game.Core;
using Game.Enemy;
using Game.Player;
using Game.GamePlay;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab, explosionPrefab;
    private CharacterController2D playerCharacter;
    private Vector2 targetPetEnemy = Vector2.zero;
    private PlayerHealth playerHealth;
    private PetAI petAI;

    private void Awake()
    {
        playerCharacter = CharacterController2D.instance;
        playerHealth = PlayerHealth.instance;
        petAI = PetAI.instance;
    }

    private void OnEnable()
    {
        if (petAI)
        {
            targetPetEnemy = (petAI.closestEnemy.position - transform.position).normalized;
        }

        StartCoroutine(TemporarilyDeactivate(1.7f));
        if (body.isKinematic)
        {
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
            }
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
                else if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletEnemy")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Player_Bullet_Explosion_1");
                    }
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
                else if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletEnemy")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Player_Bullet_Explosion_1");
                    }
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
                else if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletEnemy")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Player_Bullet_Explosion_1");
                    }
                }

                break;
            case EnemyType.Player:
                if (other.gameObject.CompareTag("ground"))
                {
                    PlayerExplosions();
                }
                else if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(playerCharacter.playerData.damageAttack);
                    PlayerExplosions();
                }
                else if (other.CompareTag("Box"))
                {
                    Destroy(other.gameObject);
                }
                else if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletPlayer")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Player_Bullet_Explosion_1");
                    }
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
                else if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletPlayer")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Player_Bullet_Explosion_1");
                    }
                }

                break;
        }
    }

    private Vector2 SetAngleSNinja()
    {
        return (playerCharacter.transform.position - transform.position).normalized;
    }

    private void PlayerExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        AudioManager.instance.Play("Player_Bullet_Explosion_1");
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(TemporarilyDeactivate(1.7f));
    }

    private void EnemyExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        AudioManager.instance.Play("Player_Bullet_Explosion_1");
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(TemporarilyDeactivate(1.7f));
    }

    private void BulletExplosions()
    {
        bulletPrefab.SetActive(false);
        explosionPrefab.SetActive(true);
        body.bodyType = RigidbodyType2D.Static;
        StartCoroutine(TemporarilyDeactivate(1.7f));
    }

    private System.Collections.IEnumerator TemporarilyDeactivate(float delay)
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