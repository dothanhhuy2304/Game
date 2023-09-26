using System.Collections;
using Photon.Pun;
using UnityEngine;
using Script.Player;

namespace Script.GamePlay
{
    public class Chest : MonoBehaviourPun
    {
        [SerializeField] private GameObject uIGuild, itemScore, itemHurt;
        private bool _isOpen;
        [SerializeField] private Animator animator;
        [SerializeField] private TMPro.TextMeshProUGUI txtValueItem;
        private int _value;
        private static readonly int IsOpen = Animator.StringToHash("isOpen");

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isOpen)
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                uIGuild.SetActive(true);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && Input.GetKey(KeyCode.F) && !_isOpen)
            {
                animator.SetBool(IsOpen, true);
                uIGuild.SetActive(false);
                //set value
                _value = Random.Range(0, 10);
                StartCoroutine(ActiveItem(3f));
                if (_value != 0)
                {
                    txtValueItem.text = "x" + _value.ToString(System.Globalization.CultureInfo.CurrentCulture);
                    GameManager.instance.SetDiamond(_value);
                }
                else
                {
                    other.GetComponent<PlayerHealth>().GetDamage(20f);
                }

                _isOpen = true;
            }
        }

        private IEnumerator ActiveItem(float delay)
        {
            if (_value != 0)
            {
                itemScore.SetActive(true);
                yield return new WaitForSeconds(delay);
                itemScore.SetActive(false);
            }
            else
            {
                itemHurt.SetActive(true);
                yield return new WaitForSeconds(delay);
                itemHurt.SetActive(false);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                uIGuild.SetActive(false);
            }
        }
    }
}