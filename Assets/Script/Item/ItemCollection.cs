using System;
using System.Collections;
using Game.GamePlay;
using UnityEngine;

namespace Game.Item
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] private ScoreData scoreData;
        [SerializeField] private States states;
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemData itemData;
        private const float Volume = 1f;
        [SerializeField] private GameObject itemObj, effectCollectedObj;
        [SerializeField] private Collider2D itemCollider;
        private PlayerHealth playerHealthBar;
        private GameManager gameManager;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
            playerHealthBar = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
            scoreData.currentScore = 0f;
            gameManager.SetScore(scoreData.currentScore);
            gameManager.SetDiamond(scoreData.diamond);
            gameManager.SetMoney(scoreData.money);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            switch (states)
            {
                case States.Score:
                {
                    switch (itemType)
                    {
                        case ItemType.Money:
                            scoreData.money += itemData.moneyReceive;
                            scoreData.currentScore += itemData.scoreReceive;
                            gameManager.SetScore(scoreData.currentScore);
                            gameManager.SetMoney(scoreData.money);
                            ScoreAndDiamondItems();

                            break;
                        case ItemType.Diamond:
                            scoreData.currentScore += itemData.scoreReceive;
                            scoreData.diamond += itemData.diamondReceive;
                            gameManager.SetDiamond(scoreData.diamond);
                            ScoreAndDiamondItems();
                            break;
                        case ItemType.None:
                        {
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                }

                case States.Heal:
                {

                    HealItems(itemData.valueReceive);
                    break;
                }
                case States.Hurt:
                {
                    HurtItems(itemData.valueReceive);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HurtItems(float value)
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            playerHealthBar.GetDamage(value);
            AudioSource.PlayClipAtPoint(itemData.soundHurtCollection, transform.position, Volume);
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private void HealItems(float value)
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            playerHealthBar.Heal(value);
            AudioSource.PlayClipAtPoint(itemData.soundCollection, transform.position, Volume);
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private void ScoreAndDiamondItems()
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            AudioSource.PlayClipAtPoint(itemData.soundCollection, transform.position, Volume);
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private IEnumerator TemporarilyDeactivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (scoreData.currentScore > scoreData.highScore)
            {
                scoreData.highScore = scoreData.currentScore;
            }
        }
    }

    public enum States
    {
        Heal,
        Hurt,
        Score
    }

    public enum ItemType
    {
        Money,
        Diamond,
        None
    }
}
