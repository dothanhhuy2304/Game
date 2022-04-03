using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Enemy
{
    public class EnemyHealth : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private Data enemyData;
        [SerializeField] private EnemyHealthBar enemyHealthBar;
        [SerializeField] private float timeRespawn;
        [SerializeField] private Collider2D enemyCollider;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            SetMaxHealth(enemyData.heathDefault, enemyData.hpIc);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool EnemyDeath()
        {
            return enemyData.currentHealth <= 0f;
        }

        private void SetMaxHealth(float maxHealth, float hpIc)
        {
            enemyData.maxHealth = maxHealth + hpIc;
            enemyData.currentHealth = enemyData.maxHealth;
            enemyHealthBar.SetHealth(enemyData.currentHealth, enemyData.maxHealth);
        }

        public void GetDamage(float damage)
        {
            enemyData.currentHealth = Mathf.Clamp(enemyData.currentHealth - damage, 0f, enemyData.maxHealth);
            if (enemyData.currentHealth <= 0) Die();
            enemyHealthBar.SetHealth(enemyData.currentHealth, enemyData.maxHealth);
        }

        public void Heal(float value)
        {
            enemyData.currentHealth =
                Mathf.Clamp(enemyData.currentHealth + value, 0f, enemyData.maxHealth);
            if (enemyData.currentHealth > enemyData.maxHealth)
                enemyData.currentHealth = enemyData.maxHealth;
            enemyHealthBar.SetHealth(enemyData.currentHealth, enemyData.maxHealth);
        }

        public void Die()
        {
            enemyData.currentHealth = 0f;
            spriteRenderer.enabled = false;
            enemyCollider.enabled = false;
            StartCoroutine(nameof(Respawn), timeRespawn);
        }

        private IEnumerator Respawn(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
            SetMaxHealth(enemyData.heathDefault, enemyData.hpIc);
            spriteRenderer.enabled = true;
            enemyCollider.enabled = true;
            yield return null;
        }
    }
}