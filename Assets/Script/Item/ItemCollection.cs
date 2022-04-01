using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Game.Item
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] private ScoreData scoreData;
        [SerializeField] private States states;
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemData itemData;
        [Range(0, 1)] public float volume = 1f;
        [SerializeField] private GameObject itemObj, effectCollectedObj;
        [SerializeField] private Collider2D itemCollider;
        private PlayerHealth playerHealthBar;
        [SerializeField] private TextMeshProUGUI txtScore;
        [SerializeField] private TextMeshProUGUI txtDiamond;
        [SerializeField] private TextMeshProUGUI txtMoney;

        private void Awake()
        {
            txtScore.text = scoreData.currentScore.ToString(CultureInfo.InvariantCulture);
            txtDiamond.text = scoreData.diamond.ToString(CultureInfo.CurrentCulture);
            txtMoney.text = scoreData.money.ToString(CultureInfo.CurrentCulture) + " $";
            scoreData.currentScore = 0f;
            playerHealthBar = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
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
                            txtScore.text = scoreData.currentScore.ToString(CultureInfo.InvariantCulture);
                            txtMoney.text = scoreData.money.ToString(CultureInfo.CurrentCulture) + " $";
                            ScoreAndDiamondItems();

                            break;
                        case ItemType.Diamond:
                            scoreData.currentScore += itemData.scoreReceive;
                            scoreData.diamond += itemData.diamondReceive;
                            txtDiamond.text = scoreData.diamond.ToString(CultureInfo.InvariantCulture);
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
            AudioSource.PlayClipAtPoint(itemData.soundHurtCollection, transform.position, volume);
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private void HealItems(float value)
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            playerHealthBar.Heal(value);
            AudioSource.PlayClipAtPoint(itemData.soundCollection, transform.position, volume);
            itemCollider.enabled = false;
            StartCoroutine(nameof(TemporarilyDeactivate), .8f);
        }

        private void ScoreAndDiamondItems()
        {
            itemObj.SetActive(false);
            effectCollectedObj.SetActive(true);
            AudioSource.PlayClipAtPoint(itemData.soundCollection, transform.position, volume);
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
