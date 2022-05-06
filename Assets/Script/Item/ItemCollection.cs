using System;
using System.Collections;
using Game.GamePlay;
using Game.Player;
using UnityEngine;

namespace Game.Item
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemData itemData;
        [SerializeField] private Collider2D itemCollider;
        [SerializeField] private Animator animator;
        private PlayerHealth playerHealth;
        private GameManager gameManager;
        private PlayerAudio playerAudio;

        private void Start()
        {
            gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
            playerHealth = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
            playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (playerHealth.PlayerIsDeath()) return;
            if (!other.CompareTag("Player")) return;
            switch (itemType)
            {
                case ItemType.Money:
                {
                    gameManager.SetScore(itemData.scoreReceive);
                    gameManager.SetMoney(itemData.moneyReceive);
                    ScoreAndDiamondItems();
                    break;
                }
                case ItemType.Diamond:
                {
                    gameManager.SetScore(itemData.scoreReceive);
                    gameManager.SetDiamond(itemData.diamondReceive);
                    ScoreAndDiamondItems();
                    break;
                }
                case ItemType.Heal:
                {
                    HealItems(itemData.valueReceive);
                    break;
                }
                case ItemType.Hurt:
                {
                    HurtItems(itemData.valueReceive);
                    break;
                }
                default:
                {
                    throw new Exception();
                }
            }
        }

        private void HurtItems(float value)
        {
            animator.SetLayerWeight(1, 1);
            playerHealth.GetDamage(value);
            playerAudio.Play("Item_Hurt");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), 0.8f);
        }

        private void HealItems(float value)
        {
            animator.SetLayerWeight(1, 1);
            playerHealth.Heal(value);
            playerAudio.Play("Item_Heal");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), 0.8f);
        }

        private void ScoreAndDiamondItems()
        {
            animator.SetLayerWeight(1, 1);
            playerAudio.Play("Item_Heal");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), 0.8f);
        }

        private IEnumerator TemporarilyDeactivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetLayerWeight(0, 1);
            gameObject.SetActive(false);
        }
    }

    public enum ItemType
    {
        Money,
        Diamond,
        Heal,
        Hurt
    }
}
