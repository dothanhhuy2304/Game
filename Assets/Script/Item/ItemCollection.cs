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

        private void Start()
        {
            playerHealth = PlayerHealth.instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (HuyManager.PlayerIsDeath()) return;
            if (!other.CompareTag("Player")) return;
            switch (itemType)
            {
                case ItemType.Money:
                {
                    GameManager.instance.SetScore(itemData.scoreReceive);
                    GameManager.instance.SetMoney(itemData.moneyReceive);
                    ScoreAndDiamondItems();
                    break;
                }
                case ItemType.Diamond:
                {
                    GameManager.instance.SetScore(itemData.scoreReceive);
                    GameManager.instance.SetDiamond(itemData.diamondReceive);
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
            AudioManager.instance.Play("Item_Hurt");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), 0.8f);
        }

        private void HealItems(float value)
        {
            animator.SetLayerWeight(1, 1);
            playerHealth.Heal(value);
            AudioManager.instance.Play("Item_Heal");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), 0.8f);
        }

        private void ScoreAndDiamondItems()
        {
            animator.SetLayerWeight(1, 1);
            AudioManager.instance.Play("Item_Heal");
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
