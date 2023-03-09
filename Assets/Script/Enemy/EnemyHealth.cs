using System.Collections;
using System.Globalization;
using DG.Tweening;
using UnityEngine;
using Game.Core;
using Game.GamePlay;

namespace Game.Enemy
{
    public class EnemyHealth : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private bool canRespawn;
        [SerializeField] private float heathDefault;
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth;
        [SerializeField] private float hpIc;
        [SerializeField] private EnemyHealthBar enemyHealthBar;
        [SerializeField] private float timeRespawn;
        [SerializeField] private Collider2D enemyCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject uIDamageEnemy;
        private TMPro.TextMeshProUGUI txtDamage;

        private void Start()
        {
            SetMaxHealth(heathDefault, hpIc);
            txtDamage = uIDamageEnemy.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        private void SetMaxHealth(float maxHealths, float hpIcs)
        {
            maxHealth = maxHealths + hpIcs;
            currentHealth = maxHealth;
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
        }

        public void GetDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
            if (currentHealth <= 0) Die();
            txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
            GameObject uIDamageInstance = Instantiate(uIDamageEnemy, transform.position + Vector3.up, Quaternion.identity);
            Destroy(uIDamageInstance, 0.5f);
        }

        public void Heal(float value)
        {
            currentHealth = Mathf.Clamp(currentHealth + value, 0f, maxHealth);
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
        }

        public bool EnemyDeath()
        {
            return currentHealth <= 0f;
        }

        public void Die()
        {
            currentHealth = 0f;
            spriteRenderer.enabled = false;
            enemyCollider.enabled = false;
            AudioManager.instance.Play("Enemy_Death");
            if (canRespawn)
            {
                ReSpawn(timeRespawn);
            }
        }

        public void ResetHeathDefault()
        {
            currentHealth = maxHealth;
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
        }

        public void EnemyReSpawn()
        {
            if (canRespawn)
            {
                ReSpawn(timeRespawn);
            }
        }

        public void ReSpawn(float timeDelay)
        {
            DOTween.Sequence().AppendInterval(timeDelay)
                .AppendCallback(() =>
                {
                    SetMaxHealth(heathDefault, hpIc);
                    spriteRenderer.enabled = true;
                    enemyCollider.enabled = true;
                }).Play();
        }
    }
}