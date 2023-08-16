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
    public class PlayerHealth :MonoBehaviourPunCallbacks, IHealthSystem
    {
        [SerializeField] private CharacterController2D playerCharacter;
        [SerializeField] private PlayerHealthBar playerHealthBar;
        [SerializeField] private PetAI petAi;
        [SerializeField] private string prefabDamagePlayer;
        //private TextMeshProUGUI _txtDamage;

        private void Start()
        {
            if (photonView.IsMine)
            {
                petAi = PetAI.IsLocalPet;
                if (playerCharacter.playerData.currentHealth <= 0)
                {
                    photonView.RPC(nameof(LoadHeath), RpcTarget.AllBuffered);
                }
                else
                {
                    photonView.RPC(nameof(LoadCurrentHealth), RpcTarget.AllBuffered);
                }

                photonView.RPC(nameof(RpcPlayerDeath), RpcTarget.AllBuffered);
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

        [PunRPC]
        private void RpcPlayerDeath()
        {
            HuyManager.Instance.SetPlayerIsDeath(0);
            HuyManager.Instance.SetPlayerIsHurt(0);
        }

        public void RpcGetDamage(float damage)
        {
            photonView.RPC(nameof(GetDamage), RpcTarget.AllBuffered, damage);
        }

        [PunRPC]
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

            var objectDamage = PhotonNetwork.Instantiate(prefabDamagePlayer, transform.position + Vector3.up, Quaternion.identity);
            TMP_Text txtDamage = objectDamage.GetComponentInChildren<TMP_Text>();
            txtDamage.text = damage.ToString(CultureInfo.CurrentCulture);
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .AppendCallback(() => { PhotonNetwork.Destroy(objectDamage); });
        }

        public void RpcHealing(float value)
        {
            photonView.RPC(nameof(Healing), RpcTarget.AllBuffered, value);
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
            HuyManager.Instance.eventResetWhenPlayerDeath?.Invoke();
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    HuyManager.Instance.SetPlayerIsDeath(1);
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
                    LoadHeath();
                    Transform position = transform;
                    position.position = new Vector3(HuyManager.Instance.currentPosition[0],
                        HuyManager.Instance.currentPosition[1], HuyManager.Instance.currentPosition[2]);
                    petAi.transform.position = position.up;
                    playerCharacter.animator.SetLayerWeight(1, 0);
                    playerCharacter.body.bodyType = RigidbodyType2D.Dynamic;
                    playerCharacter.col.enabled = true;
                    HuyManager.Instance.SetPlayerIsDeath(0);
                    Car.instance.eventResetCar?.Invoke();
                }).Play();
        }

        public void RpcDieByFalling()
        {
            photonView.RPC(nameof(DieByFalling), RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void DieByFalling()
        {
            HuyManager.Instance.eventResetWhenPlayerDeath?.Invoke();
            DOTween.Sequence()
                .AppendCallback(() =>
                {
                    HuyManager.Instance.SetPlayerIsDeath(1);
                    playerCharacter.playerData.currentHealth = 0f;
                    AudioManager.instance.Play("Enemy_Death");
                    GameManager.instance.numberScore = 0;
                    GameManager.instance.SetScore(0);
                }).AppendInterval(3)
                .AppendCallback(() =>
                {
                    LoadHeath();
                    Transform position = transform;
                    position.position = new Vector3(HuyManager.Instance.currentPosition[0],
                        HuyManager.Instance.currentPosition[1], HuyManager.Instance.currentPosition[2]);
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
                    HuyManager.Instance.SetPlayerIsHurt(1);
                    PlayerHurtAnim(playerCharacter.animator);
                    playerCharacter.body.bodyType = RigidbodyType2D.Static;
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