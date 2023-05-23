using System.Globalization;
using DG.Tweening;
using Script.Core;
using Script.GamePlay;
using TMPro;
using UnityEngine;
using Script.Enemy;
namespace Script.Player
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
            HuyManager.Instance.SetPlayerIsDeath(0);
            HuyManager.Instance.SetPlayerIsHurt(0);
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
                    //set score
                    playerCharacter.playerData.currentHealth = 0f;
                    PlayerDeathAnim(playerCharacter.animator);
                    gameManager.numberScore = 0;
                    gameManager.SetScore(0);
                    //end set score
                    HuyManager.Instance.SetPlayerIsDeath(1);
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    playerCharacter.col.enabled = false;
                    playerCharacter.animator.SetLayerWeight(1, 1f);
                    HuyManager.Instance.eventResetWhenPlayerDeath?.Invoke();
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
                    Transform position = transform;
                    position.position = new Vector3(UserPref.currentPosition[0], UserPref.currentPosition[1], UserPref.currentPosition[2]);
                    petAi.transform.position = position.up;
                    playerCharacter.animator.SetLayerWeight(1, 0);
                    playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                    playerCharacter.col.enabled = true;
                    HuyManager.Instance.SetPlayerIsDeath(0);
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }

        public void DieByFalling()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    playerCharacter.playerData.currentHealth = 0f;
                    AudioManager.instance.Play("Enemy_Death");
                    HuyManager.Instance.SetPlayerIsDeath(1);
                    gameManager.numberScore = 0;
                    gameManager.SetScore(0);
                    HuyManager.Instance.eventResetWhenPlayerDeath?.Invoke();
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
                    Transform position = transform;
                    position.position = new Vector3(UserPref.currentPosition[0], UserPref.currentPosition[1], UserPref.currentPosition[2]);
                    petAi.transform.position = position.up;
                    HuyManager.Instance.SetPlayerIsDeath(0);
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }

        private void PlayerHurt()
        {
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    PlayerHurtAnim(playerCharacter.animator);
                    HuyManager.Instance.SetPlayerIsHurt(1);
                }).AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                    HuyManager.Instance.SetPlayerIsHurt(0);
                }).Play();
        }

        private static void PlayerDeathAnim(Animator animator)
        {
            animator.SetTrigger("is_Death");
            AudioManager.instance.Play("Enemy_Death");
        }

        private static void PlayerHurtAnim(Animator animator)
        {
            animator.SetTrigger("is_Hurt");
            AudioManager.instance.Play("Player_Hurt");
        }

        private void OnApplicationQuit()
        {
            playerCharacter.playerData.currentHealth = 0f;
        }
    }
}