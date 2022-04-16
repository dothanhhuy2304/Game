using System.Collections;
using System.Globalization;
using Game.GamePlay;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private GameObject uIGuild, item;
    private bool isOpen;
    [SerializeField] private Animator animator;
    private GameManager gameManager;
    [SerializeField] private TMPro.TextMeshProUGUI txtValueItem;
    private int value = 10;
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
        //
        value = Random.Range(0, 10);
        txtValueItem.text = "x" + value.ToString(CultureInfo.CurrentCulture);
        StartCoroutine(nameof(ActiveItem), 3f);
        scoreData.diamond += value;
        gameManager.SetDiamond(scoreData.diamond);
        playerAudio.Plays_20("Chest");
        isOpen = true;
    }

    private IEnumerator ActiveItem(float delay)
    {
        yield return new WaitForSeconds(0.01f);
        item.SetActive(true);
        yield return new WaitForSeconds(delay);
        item.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        uIGuild.SetActive(false);
    }
}