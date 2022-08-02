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
                LoadCurrentHealth();
            }
            HuyManager.SetPlayerIsDeath(0);
        }

        private void SetMaxHealth(float maxHealth, float hpIc)
        {
            playerCharacter.playerData.maxHealth = maxHealth + hpIc;
            playerCharacter.playerData.currentHealth = playerCharacter.playerData.maxHealth;
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
        }

        private void LoadCurrentHealth()
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
            else
            {
                Die();
            }

            //if (playerCharacter.playerData.currentHealth <= 0) Die();
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
            PlayAnimPlayerDeath(playerCharacter.animator);

            //save score when player are death
            if (scoreData.scoreDataObj.currentScore > scoreData.scoreDataObj.highScore)
            {
                scoreData.scoreDataObj.highScore = scoreData.scoreDataObj.currentScore;
            }

            StartCoroutine(EventPlayerDeath(playerCharacter.body, playerCharacter.col, playerCharacter.animator, 3f));
        }

        private static void PlayAnimPlayerDeath(Animator animator)
        {
            animator.SetTrigger("is_Death");
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

            StartCoroutine(TimeDeathByFalling(3f));
        }

        private IEnumerator TimeDeathByFalling(float delay)
        {
            yield return new WaitForSeconds(delay);
            SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
            Transform position = transform;
            position.position = new Vector3(playerDatas.playerDataObj.position[0], playerDatas.playerDataObj.position[1], playerDatas.playerDataObj.position[2]);
            petAI.transform.position = position.up;
            HuyManager.SetPlayerIsDeath(0);
            //rest environment when player death
            Car.instance.eventResetCar?.Invoke();
        }

        private IEnumerator EventPlayerDeath(Rigidbody2D body, Collider2D col, Animator animator, float durationRespawn)
        {
            HuyManager.SetPlayerIsDeath(1);
            body.bodyType = RigidbodyType2D.Static;
            col.enabled = false;
            animator.SetLayerWeight(1, 1f);
            AudioManager.instance.Play("Enemy_Death");
            yield return new WaitForSeconds(durationRespawn);
            SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
            Transform position = transform;
            position.position = new Vector3(playerDatas.playerDataObj.position[0], playerDatas.playerDataObj.position[1], playerDatas.playerDataObj.position[2]);
            petAI.transform.position = position.up;
            animator.SetLayerWeight(1, 0);
            col.enabled = true;
            body.bodyType = RigidbodyType2D.Dynamic;
            yield return new WaitForSeconds(0.2f);
            HuyManager.SetPlayerIsDeath(0);
            //rest environment when player death
            Car.instance.eventResetCar?.Invoke();
        }

        private void PlayerHurt()
        {
            playerCharacter.animator.SetTrigger("is_Hurt");
            playerCharacter.body.bodyType = RigidbodyType2D.Static;
            HuyManager.SetPlayerIsHurt(1);
            AudioManager.instance.Play("Player_Hurt");
            StartCoroutine(Hurting(0.5f));
        }

        private IEnumerator Hurting(float delay)
        {
            yield return new WaitForSeconds(delay);
            playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
            HuyManager.SetPlayerIsHurt(0);
        }
        
        private void OnApplicationQuit()
        {
            playerCharacter.playerData.currentHealth = 0f;
        }
    }
}