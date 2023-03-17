using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Game.Enemy;
using Game.Player;
using Game.GamePlay;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private EnemyType enemyType;
    public float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem explosionFxObj;
    [SerializeField] private GameObject explosionSpriteFxObj;
    private CharacterController2D playerCharacter;
    private Vector2 targetPetEnemy = Vector2.zero;
    private PlayerHealth playerHealth;
    private PetAI petAI;
    [Header("Arc")]
    [SerializeField] private float arcHeight = 1;
    private Vector3 startPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    
    private void Awake()
    {
        playerCharacter = CharacterController2D.instance;
        playerHealth = PlayerHealth.instance;
        petAI = PetAI.instance;
    }

    private void OnEnable()
    {
        if (enemyType == EnemyType.Trunk)
        {
            startPos = transform.position;
            targetPos = playerCharacter.transform.position;
        }
    }

    private void Update()
    {
        if (enemyType == EnemyType.Trunk)
        {
            float x0 = startPos.x;
            float x1 = targetPos.x;
            float dist = x1 - x0;
            float nextX = Mathf.MoveTowards(transform.position.x, x1, bulletSpeed * Time.deltaTime);
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
            float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);
            // Rotate to face the next position, and then move there
            transform.rotation = LookAt2D(nextPos - transform.position);
            transform.position = nextPos;
            if (nextPos == targetPos)
            {
                Arrived();
            }
        }
    }

    private void Arrived()
    {
        bulletPrefab.SetActive(false);
        StartCoroutine(nameof(TemporarilyDeactivate), 1.7f);
    }

    private static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
    //end trunk

    private void BulletDirection(Transform trans)
    {
        if (petAI)
        {
            targetPetEnemy = (petAI.closestEnemy.position - transform.position).normalized;
        }

        TemporarilyDeactivate(1700);
        if (body.isKinematic)
        {
            switch (enemyType)
            {
                case EnemyType.Ninja:
                {
                    body.velocity = GetTarget(trans) * bulletSpeed;
                    break;
                }
                case EnemyType.CarnivorousPlant:
                {
                    body.velocity = trans.right * bulletSpeed;
                    break;
                }
                case EnemyType.Player:
                {
                    body.velocity = trans.right * bulletSpeed;
                    break;
                }
                case EnemyType.Pet:
                {
                    body.velocity = targetPetEnemy * bulletSpeed;
                    break;
                }
                case EnemyType.Bee:
                    body.velocity = GetTarget(trans) * bulletSpeed;
                    break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (enemyType)
        {
            case EnemyType.Ninja:
                if (other.CompareTag("ground"))
                {
                    BulletExplosions();
                }

                if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(20f);
                    BulletExplosions();
                }

                if (other.CompareTag("Bullet"))
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
                    BulletExplosions();
                }

                if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(14f);
                    BulletExplosions();
                }

                if (other.CompareTag("Bullet"))
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
                    BulletExplosions();
                }

                if (other.CompareTag("Player"))
                {
                    playerHealth.GetDamage(18f);
                    BulletExplosions();
                }

                if (other.CompareTag("Bullet"))
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
                    BulletExplosions();
                }

                if (other.CompareTag("Enemy"))
                {
                    other.GetComponent<EnemyHealth>().GetDamage(playerCharacter.playerData.damageAttack);
                    BulletExplosions();
                }

                if (other.CompareTag("Box"))
                {
                    Destroy(other.gameObject);
                }

                if (other.CompareTag("Bullet"))
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
                    BulletExplosions();
                }

                if (other.CompareTag("Bullet"))
                {
                    BulletExplosions();
                }

                if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletPlayer")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Player_Bullet_Explosion_1");
                    }
                }

                break;
            case EnemyType.Trunk:
                if (other.CompareTag("Player"))
                {
                    PlayerHealth.instance.GetDamage(20f);
                    BulletExplosions();
                    AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                }

                if (other.CompareTag("ground"))
                {
                    Arrived();
                    AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                }

                if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletEnemy")))
                    {
                        Arrived();
                        AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                    }
                }

                break;
        }
    }

    private Vector2 GetTarget(Transform trans)
    {
        return (playerCharacter.transform.position - trans.position).normalized;
    }

    private void BulletExplosions()
    {
        bulletPrefab.SetActive(false);
        if (explosionFxObj)
        {
            explosionFxObj.gameObject.SetActive(true);
            explosionFxObj.Play();
        }
        else
        {
            explosionSpriteFxObj.SetActive(true);
        }

        AudioManager.instance.Play("Player_Bullet_Explosion_1");
        body.bodyType = RigidbodyType2D.Static;
        TemporarilyDeactivate(1700);
    }

    private async void TemporarilyDeactivate(int delay)
    {
        await Task.Delay(delay);
        gameObject.SetActive(false);
        body.bodyType = RigidbodyType2D.Kinematic;
    }

    public void Shoot(Transform trans)
    {
        gameObject.SetActive(true);
        bulletPrefab.SetActive(true);
        BulletDirection(trans);
        if (explosionFxObj)
        {
            explosionFxObj.gameObject.SetActive(false);
        }
        else
        {
            explosionSpriteFxObj.SetActive(false);
        }
    }

}