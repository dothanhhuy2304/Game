using System;
using System.Collections;
using Game.GamePlay;
using UnityEngine;

namespace Game.Item
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemData itemData;
        [SerializeField] private GameObject itemObj, effectCollectedObj;
        [SerializeField] private Collider2D itemCollider;
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
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void HurtItems(float value)
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            playerHealth.GetDamage(value);
            playerAudio.Plays_20("Item_Hurt");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private void HealItems(float value)
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            playerHealth.Heal(value);
            playerAudio.Plays_20("Item_Heal");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private void ScoreAndDiamondItems()
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            playerAudio.Plays_20("Item_Heal");
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private IEnumerator TemporarilyDeactivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }

        // private void OnDisable()
        // {
        //     if (scoreData.currentScore > scoreData.highScore)
        //     {
        //         scoreData.highScore = scoreData.currentScore;
        //     }
        // }
    }

    public enum ItemType
    {
        Money,
        Diamond,
        Heal,
        Hurt
    }
}
