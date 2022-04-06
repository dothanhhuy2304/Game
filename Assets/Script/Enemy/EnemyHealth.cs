using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Enemy
{
    public class EnemyHealth : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private float heathDefault;
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth;
        [SerializeField] private float hpIc;
        [SerializeField] private EnemyHealthBar enemyHealthBar;
        [SerializeField] private float timeRespawn;
        [SerializeField] private Collider2D enemyCollider;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            SetMaxHealth(this.heathDefault, this.hpIc);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool EnemyDeath()
        {
            return currentHealth <= 0f;
        }

        private void SetMaxHealth(float maxHealths, float hpIcs)
        {
            this.maxHealth = maxHealths + hpIcs;
            this.currentHealth = this.maxHealth;
            this.enemyHealthBar.SetHealth(this.currentHealth, this.maxHealth);
        }

        public void GetDamage(float damage)
        {
            this.currentHealth = Mathf.Clamp(this.currentHealth - damage, 0f, maxHealth);
            if (this.currentHealth <= 0) Die();
            this.enemyHealthBar.SetHealth(this.currentHealth, this.maxHealth);
        }

        public void Heal(float value)
        {
            this.currentHealth = Mathf.Clamp(this.currentHealth + value, 0f, this.maxHealth);
            if (this.currentHealth > this.maxHealth)
                this.currentHealth = this.maxHealth;
            this.enemyHealthBar.SetHealth(this.currentHealth, this.maxHealth);
        }

        public void Die()
        {
            this.currentHealth = 0f;
            spriteRenderer.enabled = false;
            enemyCollider.enabled = false;
            StartCoroutine(nameof(Respawn), timeRespawn);
        }

        private IEnumerator Respawn(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
            SetMaxHealth(this.heathDefault, this.hpIc);
            spriteRenderer.enabled = true;
            enemyCollider.enabled = true;
            yield return null;
        }
    }
}