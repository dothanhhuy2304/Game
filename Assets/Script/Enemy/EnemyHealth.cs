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
            this.SetMaxHealth(this.enemyData.heathDefault, this.enemyData.hpIc);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool EnemyDeath()
        {
            return this.enemyData.currentHealth <= 0;
        }

        private void SetMaxHealth(float maxHealth, float hpIc)
        {
            this.enemyData.maxHealth = maxHealth + hpIc;
            this.enemyData.currentHealth = this.enemyData.maxHealth;
            enemyHealthBar.SetHealth(this.enemyData.currentHealth, this.enemyData.maxHealth);
        }

        public void GetDamage(float damage)
        {
            this.enemyData.currentHealth =
                Mathf.Clamp(this.enemyData.currentHealth - damage, 0, this.enemyData.maxHealth);
            if (this.enemyData.currentHealth <= 0) Die();
            enemyHealthBar.SetHealth(this.enemyData.currentHealth, this.enemyData.maxHealth);
        }

        public void Heal(float value)
        {
            this.enemyData.currentHealth =
                Mathf.Clamp(this.enemyData.currentHealth + value, 0, this.enemyData.maxHealth);
            if (this.enemyData.currentHealth > this.enemyData.maxHealth)
                this.enemyData.currentHealth = this.enemyData.maxHealth;
            enemyHealthBar.SetHealth(this.enemyData.currentHealth, this.enemyData.maxHealth);
        }

        public void Die()
        {
            this.enemyData.currentHealth = 0f;
            spriteRenderer.enabled = false;
            enemyCollider.enabled = false;
            StartCoroutine(nameof(Respawn), timeRespawn);
        }

        private IEnumerator Respawn(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
            SetMaxHealth(this.enemyData.heathDefault, this.enemyData.hpIc);
            spriteRenderer.enabled = true;
            enemyCollider.enabled = true;
            yield return null;
        }
    }
}