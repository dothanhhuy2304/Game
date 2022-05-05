using System.Collections;
using System.Globalization;
using UnityEngine;
using Game.Core;

namespace Game.Player
{
    public class PlayerHealth : MonoBehaviour, IHealthSystem
    {
        [SerializeField] public Data playerData;
        [SerializeField] public ScoreData scoreData;
        [SerializeField] public PlayerData playerDatas;
        [SerializeField] private CharacterController2D player;
        private PlayerHealthBar playerHealthBar;
        private Transform petAI;
        [SerializeField] private GameObject uIDamagePlayer;
        private TMPro.TextMeshProUGUI txtDamage;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private Collider2D col;

        private void Start()
        {
            player = GetComponent<CharacterController2D>();
            playerHealthBar = FindObjectOfType<PlayerHealthBar>()?.GetComponent<PlayerHealthBar>();
            petAI = FindObjectOfType<PetAI>()?.transform;
            spriteRenderer = GetComponent<SpriteRenderer>();
            txtDamage = uIDamagePlayer.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (playerData.currentHealth == 0)
            {
                SetMaxHealth(playerData.heathDefault, playerData.hpIc);
            }
            else
            {
                GetCurrentHealth();
            }
        }

        private void SetMaxHealth(float maxHealth, float hpIc)
        {
            playerData.maxHealth = maxHealth + hpIc;
            playerData.currentHealth = playerData.maxHealth;
            playerHealthBar.SetHealth(playerData.currentHealth, playerData.maxHealth);
        }

        private void GetCurrentHealth()
        {
            playerHealthBar.SetHealth(playerData.currentHealth, playerData.maxHealth);
        }

        public void GetDamage(float damage)
        {
            playerData.currentHealth = Mathf.Clamp(playerData.currentHealth - damage, 0, playerData.maxHealth);
            if (playerData.currentHealth > 0)
            {
                player.PlayerHurt();
            }

            if (playerData.currentHealth <= 0) Die();
            txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
            playerHealthBar.SetHealth(playerData.currentHealth, playerData.maxHealth);
            var uIDamageInstance = Instantiate(uIDamagePlayer, transform.position + Vector3.up, Quaternion.identity);
            Destroy(uIDamageInstance, 0.5f);
        }

        public void Heal(float value)
        {
            playerData.currentHealth = Mathf.Clamp(playerData.currentHealth + value, 0f, playerData.maxHealth);
            if (playerData.currentHealth > playerData.maxHealth)
                playerData.currentHealth = playerData.maxHealth;
            playerHealthBar.SetHealth(playerData.currentHealth, playerData.maxHealth);
        }

        public void Die()
        {
            playerData.currentHealth = 0f;
            body.bodyType = RigidbodyType2D.Static;
            col.enabled = false;
            if (playerData.currentHealth == 0)
            {
                player.PlayerDeath();
            }

            //save score
            if (scoreData.currentScore > scoreData.highScore)
            {
                scoreData.highScore = scoreData.currentScore;
            }

            StartCoroutine(nameof(TimeDelayDeath), 3f);
        }

        public bool PlayerIsDeath()
        {
            return playerData.currentHealth <= 0f;
        }

        private IEnumerator TimeDelayDeath(float delay)
        {
            yield return new WaitForSeconds(.6f);
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(delay);
            // ReSharper disable once Unity.InefficientPropertyAccess
            spriteRenderer.enabled = true;
            SetMaxHealth(playerData.heathDefault, playerData.hpIc);
            var position = transform;
            position.position = playerDatas.position;
            petAI.position = position.up;
            yield return new WaitForSeconds(0.3f);
            col.enabled = true;
            body.bodyType = RigidbodyType2D.Dynamic;
            yield return null;
        }

        private void OnApplicationQuit()
        {
            playerData.currentHealth = 0f;
        }
    }
}