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
    public class PlayerHealth :MonoBehaviourPun, IHealthSystem
    {
        [SerializeField] private CharacterController2D playerCharacter;
        [SerializeField] private PlayerHealthBar playerHealthBar;
        [SerializeField] private PetAI petAi;
        [SerializeField] private string prefabDamagePlayer;
        public bool isDeath;
        public bool isHurt;

        private void Start()
        {
            if (playerCharacter.pv.IsMine)
            {
                petAi = PetAI.IsLocalPet;
                playerCharacter.pv.RPC(playerCharacter.playerData.currentHealth <= 0
                    ? nameof(LoadHeath)
                    : nameof(LoadCurrentHealth), RpcTarget.AllBuffered);
            }
        }

        [PunRPC]
        private void LoadHeath()
        {
            playerCharacter.playerData.maxHealth = playerCharacter.playerData.heathDefault + playerCharacter.playerData.hpIc;
            playerCharacter.playerData.currentHealth = playerCharacter.playerData.maxHealth;
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
        }

        [PunRPC]
        private void LoadCurrentHealth()
        {
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
        }

        public void GetDamage(float damage)
        {
            playerCharacter.playerData.currentHealth = Mathf.Clamp(playerCharacter.playerData.currentHealth - damage, 0,
                playerCharacter.playerData.maxHealth);
            if (playerCharacter.playerData.currentHealth > 0)
            {
                PlayerHurt();
            }
            else
            {
                Die();
            }

            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth, playerCharacter.playerData.maxHealth);
            if (playerCharacter.pv.IsMine)
            {
                var objectDamage = PhotonNetwork.Instantiate(prefabDamagePlayer, transform.position + Vector3.up,
                    Quaternion.identity);
                TMP_Text txtDamage = objectDamage.GetComponentInChildren<TMP_Text>();
                txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
                DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(() => { PhotonNetwork.Destroy(objectDamage); });
            }
        }

        public void RpcHealing(float value)
        {
            playerCharacter.pv.RPC(nameof(Healing), RpcTarget.AllBuffered, value);
        }

        [PunRPC]
        public void Healing(float value)
        {
            playerCharacter.playerData.currentHealth = Mathf.Clamp(playerCharacter.playerData.currentHealth + value, 0f,
                playerCharacter.playerData.maxHealth);
            if (playerCharacter.playerData.currentHealth > playerCharacter.playerData.maxHealth)
                playerCharacter.playerData.currentHealth = playerCharacter.playerData.maxHealth;
            playerHealthBar.SetHealth(playerCharacter.playerData.currentHealth,
                playerCharacter.playerData.maxHealth);
        }

        public void Die()
        {
            if (!playerCharacter.pv.IsMine)
            {
                return;
            }
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    isDeath = true;
                    playerCharacter.playerData.currentHealth = 0;
                    PlayerDeathAnim(playerCharacter.animator);
                    GameManager.instance.numberScore = 0;
                    GameManager.instance.SetScore(0);
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    playerCharacter.col.enabled = false;
                    playerCharacter.animator.SetLayerWeight(1, 1f);
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    playerCharacter.pv.RPC(nameof(LoadHeath), RpcTarget.AllBuffered);
                    Transform position = transform;
                    position.position = new Vector3(HuyManager.Instance.currentPosition[0],
                        HuyManager.Instance.currentPosition[1], HuyManager.Instance.currentPosition[2]);
                    petAi.transform.position = position.up;
                    playerCharacter.animator.SetLayerWeight(1, 0);
                    playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                    playerCharacter.col.enabled = true;
                    isDeath = false;
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
            
        }

        public void DiedFromFalling()
        {
            if (!playerCharacter.pv.IsMine)
            {
                return;
            }
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    isDeath = true;
                    playerCharacter.playerData.currentHealth = 0f;
                    AudioManager.instance.Play("Enemy_Death");
                    GameManager.instance.numberScore = 0;
                    GameManager.instance.SetScore(0);
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    playerCharacter.pv.RPC(nameof(LoadHeath), RpcTarget.AllBuffered);
                    Transform position = transform;
                    position.position = new Vector3(HuyManager.Instance.currentPosition[0],
                        HuyManager.Instance.currentPosition[1], HuyManager.Instance.currentPosition[2]);
                    petAi.transform.position = position.up;
                    isDeath = false;
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }

        private void PlayerHurt()
        {
            if (!playerCharacter.photonView.IsMine)
            {
                return;
            }

            DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        isHurt = true;
                        PlayerHurtAnim(playerCharacter.animator);
                        playerCharacter.body.bodyType = RigidbodyType2D.Static;
                    }).AppendInterval(0.5f)
                    .AppendCallback(() =>
                    {
                        playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                        isHurt = false;
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