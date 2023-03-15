using System.Collections;
using Game.GamePlay;
using UnityEngine;
using Game.Player;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject uIGuild, itemScore, itemHurt;
    private bool isOpen;
    [SerializeField] private Animator animator;
    [SerializeField] private TMPro.TextMeshProUGUI txtValueItem;
    private int value;
    private static readonly int IsOpen = Animator.StringToHash("isOpen");

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpen)
        {
            if (other.CompareTag("Player"))
            {
                uIGuild.SetActive(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isOpen)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    animator.SetBool(IsOpen, true);
                    uIGuild.SetActive(false);
                    //set value
                    value = Random.Range(0, 10);
                    StartCoroutine(ActiveItem(3f));
                    if (value != 0)
                    {
                        txtValueItem.text = "x" + value.ToString(System.Globalization.CultureInfo.CurrentCulture);
                        GameManager.instance.SetDiamond(value);
                    }
                    else
                    {
                        PlayerHealth.instance.GetDamage(20f);
                    }

                    isOpen = true;
                }
            }
        }
    }

    private IEnumerator ActiveItem(float delay)
    {
        if (value != 0)
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