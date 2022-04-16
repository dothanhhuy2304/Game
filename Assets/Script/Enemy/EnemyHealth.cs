using System.Collections;
using System.Globalization;
using UnityEngine;
using Game.Core;
using TMPro;

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
        private TextMeshProUGUI txtDamage;
        private PlayerAudio playerAudio;

        private void Start()
        {
            SetMaxHealth(this.heathDefault, this.hpIc);
            if (!spriteRenderer)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
            txtDamage = uIDamageEnemy.GetComponentInChildren<TextMeshProUGUI>();
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
            this.txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
            this.enemyHealthBar.SetHealth(this.currentHealth, this.maxHealth);
            var uIDamageInstance =
                Instantiate(this.uIDamageEnemy, transform.position + Vector3.up, Quaternion.identity);
            Destroy(uIDamageInstance, 0.5f);
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
            playerAudio.Plays_10("Enemy_Death");
            this.currentHealth = 0f;
            if (canRespawn)
            {
                this.spriteRenderer.enabled = false;
                this.enemyCollider.enabled = false;
                StartCoroutine(nameof(Respawn), timeRespawn);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        private IEnumerator Respawn(float timeDelay)
        {
            yield return new WaitForSeconds(timeDelay);
            SetMaxHealth(this.heathDefault, this.hpIc);
            this.spriteRenderer.enabled = true;
            this.enemyCollider.enabled = true;
            yield return null;
        }
    }
}