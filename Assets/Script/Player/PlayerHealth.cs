using System.Globalization;
using DG.Tweening;
using Photon.Pun;
using Script.Core;
using Script.GamePlay;
using TMPro;
using UnityEngine;
using Script.Enemy;
namespace Script.Player
{
    public class PlayerHealth : MonoBehaviourPunCallbacks
    {
        public static PlayerHealth instance;
        private GameManager gameManager;
        [SerializeField] private CharacterController2D playerCharacter;
        [SerializeField] private PlayerHealthBar playerHealthBar;
        [SerializeField] private PetAI petAi;
        [SerializeField] private GameObject uIDamagePlayer;
        private TextMeshProUGUI txtDamage;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                if (instance==null)
                {
                    instance = this;
                }
                else
                {
                    Destroy(this);
                }
            }
        }

        private void Start()
        {
            photonView.RPC(nameof(InitPlayerHealth), RpcTarget.All);
        }

        [PunRPC]
        private void InitPlayerHealth()
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
        
        [PunRPC]
        public void GetDamage(float damage)
        {
            if (photonView.IsMine)
            {
                playerCharacter.playerData.currentHealth = Mathf.Clamp(
                    playerCharacter.playerData.currentHealth - damage, 0, playerCharacter.playerData.maxHealth);
                if (playerCharacter.playerData.currentHealth > 0)
                {
                    PlayerHurt();
                }
                else
                {
                    Die();
                }

                txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
                playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth,
                    playerCharacter.playerData.maxHealth);
                var uIDamageInstance =
                    Instantiate(uIDamagePlayer, transform.position + Vector3.up, Quaternion.identity);
                Destroy(uIDamageInstance, 0.5f);
            }
        }

        public void Heal(float value)
        {
            if (photonView.IsMine)
            {
                playerCharacter.playerData.currentHealth = Mathf.Clamp(playerCharacter.playerData.currentHealth + value,
                    0f, playerCharacter.playerData.maxHealth);
                if (playerCharacter.playerData.currentHealth > playerCharacter.playerData.maxHealth)
                    playerCharacter.playerData.currentHealth = playerCharacter.playerData.maxHealth;
                playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth,
                    playerCharacter.playerData.maxHealth);
            }
        }

        private void Die()
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
                    HuyManager.SetPlayerIsDeath(1);
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    playerCharacter.col.enabled = false;
                    playerCharacter.animator.SetLayerWeight(1, 1f);
                    HuyManager.eventResetWhenPlayerDeath?.Invoke();
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
                    HuyManager.SetPlayerIsDeath(0);
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }
        
        [PunRPC]
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
                    HuyManager.eventResetWhenPlayerDeath?.Invoke();
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    SetMaxHealth(playerCharacter.playerData.heathDefault, playerCharacter.playerData.hpIc);
                    Transform position = transform;
                    position.position = new Vector3(UserPref.currentPosition[0], UserPref.currentPosition[1], UserPref.currentPosition[2]);
                    petAi.transform.position = position.up;
                    HuyManager.SetPlayerIsDeath(0);
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }

        [PunRPC]
        private void PlayerHurt()
        {
            if (photonView.IsMine)
            {
                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        playerCharacter.body.bodyType = RigidbodyType2D.Static;
                        PlayerHurtAnim(playerCharacter.animator);
                        HuyManager.SetPlayerIsHurt(1);
                    }).AppendInterval(0.5f)
                    .AppendCallback(() =>
                    {
                        playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                        HuyManager.SetPlayerIsHurt(0);
                    }).Play();
            }
        }

        private void PlayerDeathAnim(Animator animator)
        {
            if (photonView.IsMine)
            {
                animator.SetTrigger("is_Death");
                AudioManager.instance.Play("Enemy_Death");
            }
        }

        private void PlayerHurtAnim(Animator animator)
        {
            if (photonView.IsMine)
            {
                animator.SetTrigger("is_Hurt");
                AudioManager.instance.Play("Player_Hurt");
            }
        }

        private void OnApplicationQuit()
        {
            if (photonView.IsMine)
            {
                playerCharacter.playerData.currentHealth = 0f;
            }
        }
    }
}