using System.Globalization;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Player;
using TMPro;

namespace Script.Enemy
{
    public class EnemyHealth : MonoBehaviourPun
    {
        [SerializeField] private bool canReSpawn;
        [SerializeField] private float heathDefault;
        [SerializeField] private float currentHealth;
        [SerializeField] private float maxHealth;
        [SerializeField] private float hpIc;
        public float damageFix;
        [SerializeField] private EnemyHealthBar enemyHealthBar;
        [SerializeField] private float timeReSpawn;
        [SerializeField] private Collider2D enemyCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private string objectDamageEnemy;

        private void Start()
        {
            LoadHealth();
        }

        private void LoadHealth()
        {
            maxHealth = heathDefault + hpIc;
            currentHealth = maxHealth;
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
        }
        
        public void EnemyGetDamage(float damage)
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
            if (currentHealth <= 0) Die();
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
            if (photonView.IsMine)
            {
                var damageInstance = PhotonNetwork.Instantiate(objectDamageEnemy, transform.position + Vector3.up, Quaternion.identity);
                TMP_Text txtDamage = damageInstance.GetComponentInChildren<TMP_Text>();
                txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
                DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(() => { PhotonNetwork.Destroy(damageInstance); });
            }
        }

        public bool EnemyDeath()
        {
            return currentHealth <= 0f;
        }
        
        public void ResetHeathDefault()
        {
            currentHealth = maxHealth;
            enemyHealthBar.SetHealth(currentHealth, maxHealth);
        }

        private void Die()
        {
            currentHealth = 0f;
            spriteRenderer.enabled = false;
            enemyCollider.enabled = false;
            AudioManager.instance.Play("Enemy_Death");
            if (canReSpawn)
            {
                ReSpawn(timeReSpawn);
            }
        }

        public void ReSpawn(float timeDelay)
        {
            DOTween.Sequence().AppendInterval(timeDelay)
                .AppendCallback(() =>
                {
                    LoadHealth();
                    spriteRenderer.enabled = true;
                    enemyCollider.enabled = true;
                }).Play();
        }
    }
}