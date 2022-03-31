using System;
using System.Collections;
using UnityEngine;

namespace Game.Item
{
    public class ItemCollection : MonoBehaviour
    {
        [SerializeField] private States states;
        [SerializeField] private ItemData itemData;
        [Range(0, 1)] public float volume = 1f;
        [SerializeField] private GameObject itemObj, effectCollectedObj;
        [SerializeField] private Collider2D itemCollider;
        private PlayerHealth playerHealthBar;

        private void Awake()
        {
            playerHealthBar = FindObjectOfType<PlayerHealth>().GetComponent<PlayerHealth>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            switch (states)
            {
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

        private IEnumerator TemporarilyDeactivate(float delay)
        {
            yield return new WaitForSeconds(delay);
            this.gameObject.SetActive(false);
        }
    }

    public enum States
    {
        Heal,
        Hurt
    }
}
