using DG.Tweening;
using Script.GamePlay;
using Script.Player;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;

namespace Script.Item
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemData itemData;
        [SerializeField] private Collider2D itemCollider;
        [SerializeField] private Animator animator;
        private PlayerHealth playerHealth;
        private GameManager gameManager;

        private void Start()
        {
            gameManager = GameManager.instance;
            playerHealth = PlayerHealth.instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!HuyManager.Instance.PlayerIsDeath())
            {
                if (other.CompareTag("Player"))
                {
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
                    }
                }
            }
        }

        private void HurtItems(float value)
        {
            animator.SetLayerWeight(1, 1);
            playerHealth.GetDamage(value);
            AudioManager.instance.Play("Item_Hurt");
            itemCollider.enabled = false;
            TemporarilyDeactivate(0.8f);
        }

        private void HealItems(float value)
        {
            animator.SetLayerWeight(1, 1);
            playerHealth.Healing(value);
            AudioManager.instance.Play("Item_Heal");
            itemCollider.enabled = false;
            TemporarilyDeactivate(0.8f);
        }

        private void ScoreAndDiamondItems()
        {
            animator.SetLayerWeight(1, 1);
            AudioManager.instance.Play("Item_Heal");
            itemCollider.enabled = false;
            TemporarilyDeactivate(0.8f);
        }

        private void TemporarilyDeactivate(float delay)
        {
            DOTween.Sequence()
                .AppendInterval(delay)
                .AppendCallback(() =>
                {
                    animator.SetLayerWeight(0, 1);
                    gameObject.SetActive(false);
                }).Play();
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
