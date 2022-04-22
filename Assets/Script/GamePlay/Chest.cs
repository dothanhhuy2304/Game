using System.Collections;
using UnityEngine;

namespace Game.GamePlay
{

    public class Chest : MonoBehaviour
    {
        [SerializeField] private GameObject uIGuild, itemScore, itemHurt;
        private bool isOpen;
        [SerializeField] private Animator animator;
        private GameManager gameManager;
        [SerializeField] private TMPro.TextMeshProUGUI txtValueItem;
        private int value;
        private static readonly int IsOpen = Animator.StringToHash("isOpen");
        private PlayerAudio playerAudio;

        private void Awake()
        {
            gameManager = FindObjectOfType<GameManager>()?.GetComponent<GameManager>();
            playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isOpen) return;
            if (other.CompareTag("Player"))
            {
                uIGuild.SetActive(true);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (isOpen) return;
            if (!other.CompareTag("Player")) return;
            if (!Input.GetKey(KeyCode.F)) return;
            animator.SetBool(IsOpen, true);
            uIGuild.SetActive(false);
            //value
            value = Random.Range(0, 10);
            StartCoroutine(nameof(ActiveItem), 3f);
            if (value != 0)
            {
                txtValueItem.text = "x" + value.ToString(System.Globalization.CultureInfo.CurrentCulture);
                gameManager.SetDiamond(value);
                playerAudio.Plays_20("Chest");
            }
            else
            {
                playerAudio.Plays_20("Item_Hurt");
                other.gameObject.GetComponent<PlayerHealth>().GetDamage(20f);
            }

            isOpen = true;
        }

        private IEnumerator ActiveItem(float delay)
        {
            if (value != 0)
            {
                yield return new WaitForSeconds(0.01f);
                itemScore.SetActive(true);
                yield return new WaitForSeconds(delay);
                itemScore.SetActive(false);
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(0.01f);
                itemHurt.SetActive(true);
                yield return new WaitForSeconds(delay);
                itemHurt.SetActive(false);
                yield return null;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            uIGuild.SetActive(false);
        }
    }
}