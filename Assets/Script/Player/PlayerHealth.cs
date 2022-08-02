using System.Collections;
using System.Globalization;
using UnityEngine;
using Game.Core;
using Game.GamePlay;

namespace Game.Player
{
    public class PlayerHealth : FastSingleton<PlayerHealth>, IHealthSystem
    {
        [SerializeField] private ScoreData scoreData;
        [SerializeField] private PlayerData playerDatas;
        private CharacterController2D playerCharacter;
        private PlayerHealthBar playerHealthBar;
        private PetAI petAI;
        [SerializeField] private GameObject uIDamagePlayer;
        private TMPro.TextMeshProUGUI txtDamage;

        private void Start()
        {
            playerCharacter = CharacterController2D.instance;
            playerHealthBar = PlayerHealthBar.instance;
            petAI = PetAI.instance;
            txtDamage = uIDamagePlayer.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (playerCharacter.playerData.currentHealth == 0)
            {
                SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
            }
            else
            {
                GetCurrentHealth();
            }
            HuyManager.SetPlayerIsDeath(0);
        }

        private void SetMaxHealth(float maxHealth, float hpIc)
        {
            playerCharacter.playerData.maxHealth = maxHealth + hpIc;
            playerCharacter.playerData.currentHealth = playerCharacter.playerData.maxHealth;
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
        }

        private void GetCurrentHealth()
        {
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
        }

        public void GetDamage(float damage)
        {
            playerCharacter.playerData.currentHealth = Mathf.Clamp(playerCharacter.playerData.currentHealth - damage, 0, playerCharacter.playerData.maxHealth);
            if (playerCharacter.playerData.currentHealth > 0)
            { 
                PlayerHurt();
            }

            if (playerCharacter.playerData.currentHealth <= 0) Die();
            txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
            var uIDamageInstance = Instantiate(uIDamagePlayer, transform.position + Vector3.up, Quaternion.identity);
            Destroy(uIDamageInstance, 0.5f);
        }

        public void Heal(float value)
        {
            playerCharacter.playerData.currentHealth = Mathf.Clamp(playerCharacter.playerData.currentHealth + value, 0f, playerCharacter.playerData.maxHealth);
            if (playerCharacter.playerData.currentHealth > playerCharacter.playerData.maxHealth)
                playerCharacter.playerData.currentHealth = playerCharacter.playerData.maxHealth;
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
        }

        public void Die()
        {
            playerCharacter.playerData.currentHealth = 0f;
            if (playerCharacter.playerData.currentHealth == 0)
            {
                PlayAnimPlayerDeath();
            }

            //save score
            if (scoreData.scoreDataObj.currentScore > scoreData.scoreDataObj.highScore)
            {
                scoreData.scoreDataObj.highScore = scoreData.scoreDataObj.currentScore;
            }

            StartCoroutine(EventDeath(playerCharacter.body, playerCharacter.col, playerCharacter.animator, 3f));
        }
        
        private void PlayAnimPlayerDeath()
        {
            playerCharacter.animator.SetTrigger("is_Death");
            AudioManager.instance.Play("Enemy_Death");
        }
        

        public void DieByFalling()
        {
            playerCharacter.playerData.currentHealth = 0f;
            AudioManager.instance.Play("Enemy_Death");
            HuyManager.SetPlayerIsDeath(1);
            if (scoreData.scoreDataObj.currentScore > scoreData.scoreDataObj.highScore)
            {
                scoreData.scoreDataObj.highScore = scoreData.scoreDataObj.currentScore;
            }

            StartCoroutine(nameof(TimeDelayDeathFalling), 3f);
        }

        private IEnumerator TimeDelayDeathFalling(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
            var position = transform; position.position = new Vector3(playerDatas.playerDataObj.position[0], playerDatas.playerDataObj.position[1], playerDatas.playerDataObj.position[2]);
            petAI.transform.position = position.up;
            HuyManager.SetPlayerIsDeath(0);
        }
        
        private IEnumerator EventDeath(Rigidbody2D body, Collider2D col, Animator animator, float durationRespawn)
        {
            HuyManager.SetPlayerIsDeath(1);
            body.bodyType = RigidbodyType2D.Static;
            col.enabled = false;
            animator.SetLayerWeight(1, 1f);
            AudioManager.instance.Play("Enemy_Death");
            yield return new WaitForSeconds(0.6f);
            yield return new WaitForSeconds(durationRespawn);
            SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
            var position = transform;
            position.position = new Vector3(playerDatas.playerDataObj.position[0], playerDatas.playerDataObj.position[1], playerDatas.playerDataObj.position[2]);
            petAI.transform.position = position.up;
            animator.SetLayerWeight(1, 0);
            body.bodyType = RigidbodyType2D.Dynamic;
            yield return new WaitForSeconds(0.1f);
            HuyManager.SetPlayerIsDeath(0);
            col.enabled = true;
        }

        public void PlayerHurt()
        {
            playerCharacter.animator.SetTrigger("is_Hurt");
            playerCharacter.body.bodyType = RigidbodyType2D.Static;
            playerCharacter.isHurt = true;
            AudioManager.instance.Play("Player_Hurt");
            StartCoroutine(nameof(Hurting), 0.5f);
        }

        private IEnumerator Hurting(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
            playerCharacter.isHurt = false;
        }
        
        private void OnApplicationQuit()
        {
            playerCharacter.playerData.currentHealth = 0f;
        }
    }
}