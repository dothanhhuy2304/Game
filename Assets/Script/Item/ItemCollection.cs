using DG.Tweening;
using Photon.Pun;
using Script.GamePlay;
using Script.Player;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;

namespace Script.Item
{
    public class ItemCollection : MonoBehaviourPunCallbacks
    {
        [SerializeField] private ItemType itemType;
        [SerializeField] private ItemData itemData;
        [SerializeField] private Collider2D itemCollider;
        [SerializeField] private Animator animator;
        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.instance;
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
                            _gameManager.SetScore(itemData.scoreReceive);
                            _gameManager.SetMoney(itemData.moneyReceive);
                            animator.SetLayerWeight(1, 1);
                            AudioManager.instance.Play("Item_Heal");
                            itemCollider.enabled = false;
                            DOTween.Sequence()
                                .AppendInterval(0.8f)
                                .AppendCallback(() =>
                                {
                                    animator.SetLayerWeight(0, 1);
                                    gameObject.SetActive(false);
                                }).Play();
                            break;
                        }
                        case ItemType.Diamond:
                        {
                            _gameManager.SetScore(itemData.scoreReceive);
                            _gameManager.SetDiamond(itemData.diamondReceive);
                            animator.SetLayerWeight(1, 1);
                            AudioManager.instance.Play("Item_Heal");
                            itemCollider.enabled = false;
                            DOTween.Sequence()
                                .AppendInterval(0.8f)
                                .AppendCallback(() =>
                                {
                                    animator.SetLayerWeight(0, 1);
                                    gameObject.SetActive(false);
                                }).Play();
                            break;
                        }
                        case ItemType.Heal:
                        {
                            animator.SetLayerWeight(1, 1);
                            other.GetComponent<CharacterController2D>().playerHealth.Healing(itemData.valueReceive);
                            AudioManager.instance.Play("Item_Heal");
                            itemCollider.enabled = false;
                            DOTween.Sequence()
                                .AppendInterval(0.8f)
                                .AppendCallback(() =>
                                {
                                    animator.SetLayerWeight(0, 1);
                                    gameObject.SetActive(false);
                                }).Play();
                            break;
                        }
                        case ItemType.Hurt:
                        {
                            animator.SetLayerWeight(1, 1);
                            other.GetComponent<CharacterController2D>().playerHealth.GetDamage(itemData.valueReceive);
                            AudioManager.instance.Play("Item_Hurt");
                            itemCollider.enabled = false;
                            DOTween.Sequence()
                                .AppendInterval(0.8f)
                                .AppendCallback(() =>
                                {
                                    animator.SetLayerWeight(0, 1);
                                    gameObject.SetActive(false);
                                }).Play();
                            break;
                        }
                    }
                }
            }
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
