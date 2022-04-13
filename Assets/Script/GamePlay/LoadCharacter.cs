using Game.GamePlay;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject[] characters;

    private void Awake()
    {
        characters[playerData.characterSelection].SetActive(true);
        //var uiManager = FindObjectOfType<UIManager>().GetComponent<UIManager>();
        if (UIManager.Instance.healthUI.activeSelf || UIManager.Instance.scoreUI.activeSelf) return;
        UIManager.Instance.healthUI.SetActive(true);
        UIManager.Instance.scoreUI.SetActive(true);
        UIManager.Instance.btnBackToMenuUI.SetActive(true);
        UIManager.Instance.btnRestart.SetActive(true);
    }
}
