using System.Globalization;
using DG.Tweening;
using Game.Core;
using Game.GamePlay;
using TMPro;
using UnityEngine;

namespace Game.Player
{
    public class PlayerHealth : FastSingleton<PlayerHealth>, IHealthSystem
    {
        private GameManager gameManager;
        [SerializeField] private CharacterController2D playerCharacter;
        [SerializeField] private PlayerHealthBar playerHealthBar;
        [SerializeField] private PetAI petAi;
        [SerializeField] private GameObject uIDamagePlayer;
        private TextMeshProUGUI txtDamage;

        private void Start()
        {
            gameManager = GameManager.instance;
            txtDamage = uIDamagePlayer.GetComponentInChildren<TextMeshProUGUI>();
            if (playerCharacter.playerData.currentHealth <= 0)
            {
                SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
            }
            else
            {
                LoadCurrentHealth();
            }
            HuyManager.SetPlayerIsDeath(0);
            HuyManager.SetPlayerIsHurt(0);
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
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    playerCharacter.playerData.currentHealth = 0f;
                    PlayAnimPlayerDeath(playerCharacter.animator);
                    gameManager.numberScore = 0;
                    gameManager.SetScore(0);
                }).AppendInterval(3f)
                .AppendCallback(() =>
                {
                    HuyManager.SetPlayerIsDeath(1);
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    playerCharacter.col.enabled = false;
                    playerCharacter.animator.SetLayerWeight(1, 1f);
                    AudioManager.instance.Play("Enemy_Death");
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
                    Transform position = transform;
                    position.position = new Vector3(UserPref.currentPosition[0], UserPref.currentPosition[1], UserPref.currentPosition[2]);
                    petAi.transform.position = position.up;
                    playerCharacter.animator.SetLayerWeight(1, 0);
                    playerCharacter.col.enabled = true;
                    HuyManager.SetPlayerIsDeath(0);
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }

        private static void PlayAnimPlayerDeath(Animator animator)
        {
            animator.SetTrigger("is_Death");
            AudioManager.instance.Play("Enemy_Death");
        }

        public void DieByFalling()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    playerCharacter.playerData.currentHealth = 0f;
                    AudioManager.instance.Play("Enemy_Death");
                    HuyManager.SetPlayerIsDeath(1);
                    gameManager.numberScore = 0;
                    gameManager.SetScore(0);
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
                    Transform position = transform;
                    position.position = new Vector3(UserPref.currentPosition[0], UserPref.currentPosition[1], UserPref.currentPosition[2]);
                    petAi.transform.position = position.up;
                    HuyManager.SetPlayerIsDeath(0);
                }).Play();
        }

        private void PlayerHurt()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    playerCharacter.animator.SetTrigger("is_Hurt");
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    HuyManager.SetPlayerIsHurt(1);
                    AudioManager.instance.Play("Player_Hurt");
                }).AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                    HuyManager.SetPlayerIsHurt(0);
                }).Play();
        }

        private void OnApplicationQuit()
        {
            playerCharacter.playerData.currentHealth = 0f;
        }
    }
}