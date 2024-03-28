using DG.Tweening;
using Script.Enemy;
using Script.Player;
using UnityEngine;
using Script.Core;

public class FireProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private EnemyType enemyType;
    public float bulletSpeed = 10f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private ParticleSystem explosionFxObj;
    [SerializeField] private GameObject explosionSpriteFxObj;
    private CharacterController2D _playerCharacter;
    private PetAI _petAi;

    private void Awake()
    {
        _playerCharacter = CharacterController2D.IsLocalPlayer;
        _petAi = PetAI.IsLocalPet;
    }

    private void BulletDirection(Transform startPosition, Transform target = null)
    {
        TemporarilyDeactivate(1.7f);
        if (body.isKinematic)
        {
            switch (enemyType)
            {
                case EnemyType.Ninja:
                    body.velocity = GetDistanceObjectToPlayer(startPosition, target) * bulletSpeed;
                    break;
                case EnemyType.CarnivorousPlant:
                    body.velocity = startPosition.right * bulletSpeed;
                    break;
                case EnemyType.Player:
                    body.velocity = Vector3.right * (startPosition.localScale.x * bulletSpeed);
                    break;
                case EnemyType.Pet:
                    body.velocity = GetDistanceObjectToPlayer(startPosition, target) * bulletSpeed;
                    break;
                case EnemyType.Bee:
                    body.velocity = GetDistanceObjectToPlayer(startPosition, target) * bulletSpeed;
                    break;
                case EnemyType.Trunk:
                    body.velocity = -startPosition.right * bulletSpeed;
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
                    other.GetComponent<PlayerHealth>().GetDamage(20f);
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
                    other.GetComponent<PlayerHealth>().GetDamage(14f);
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
                    other.GetComponent<PlayerHealth>().GetDamage(18f);
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
                    EnemyHealth eHealth = other.GetComponent<EnemyHealth>();
                    eHealth.EnemyGetDamage(_playerCharacter.playerData.damageAttack + eHealth.damageFix);
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
                    EnemyHealth eHealth = other.GetComponent<EnemyHealth>();
                    eHealth.EnemyGetDamage(_petAi.petData.damageAttack + eHealth.damageFix);
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

                if (other.CompareTag("ground"))
                {
                    BulletExplosions();
                    AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                }

                break;
            case EnemyType.Trunk:
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<PlayerHealth>().GetDamage(20f);
                    BulletExplosions();
                    AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                }

                if (other.CompareTag("ground"))
                {
                    BulletExplosions();
                    AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                }

                if (other.CompareTag("Bullet"))
                {
                    if (!body.IsTouchingLayers(1 << LayerMask.NameToLayer("BulletEnemy")))
                    {
                        BulletExplosions();
                        AudioManager.instance.Play("Enemy_Bullet_Explosion_1");
                    }
                }

                break;
        }
    }

    private Vector2 GetDistanceObjectToPlayer(Transform startPosition,Transform target)
    {
        return (target.position - startPosition.position).normalized;
    }

    private void BulletExplosions()
    {
        bulletPrefab.SetActive(false);
        if (explosionFxObj)
        {
            explosionFxObj.gameObject.SetActive(true);
        }
        else
        {
            explosionSpriteFxObj.SetActive(true);
        }

        body.bodyType = RigidbodyType2D.Static;
        AudioManager.instance.Play("Player_Bullet_Explosion_1");
        TemporarilyDeactivate(1.7f);
    }

    private void TemporarilyDeactivate(float delay)
    {
        DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() =>
            {
                if (explosionFxObj)
                {
                    explosionFxObj.gameObject.SetActive(false);
                }
                else
                {
                    explosionSpriteFxObj.SetActive(false);
                }
                body.bodyType = RigidbodyType2D.Kinematic;
                gameObject.SetActive(false);
            }).Play();
    }

    public void Shoot(Transform trans, Transform target = null)
    {
        gameObject.SetActive(true);
        bulletPrefab.SetActive(true);
        BulletDirection(trans, target);
    }

}