using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private GameObject uIGuild, item;
    private bool isOpen;
    private Animator animator;
    private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI txtValueItem;
    private readonly float value = 10f;
    private static readonly int IsOpen = Animator.StringToHash("isOpen");
    private PlayerAudio playerAuudio;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        playerAuudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
    }

    private void OnEnable()
    {
        txtValueItem.text = "x" + value.ToString(CultureInfo.CurrentCulture);
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
        StartCoroutine(nameof(ActiveItem), 3f);
        scoreData.diamond += value;
        gameManager.SetDiamond(scoreData.diamond);
        playerAuudio.Plays_20("Chest");
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