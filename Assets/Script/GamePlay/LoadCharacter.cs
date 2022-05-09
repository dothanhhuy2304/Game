using Game.GamePlay;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject[] characters;

    private void Awake()
    {
        characters[playerData.playerDataObj.characterSelection].SetActive(true);
        var playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
        var uiManager = FindObjectOfType<UIManager>().GetComponent<UIManager>();
        playerAudio.Plays_Music("Music_Game");
        if (uiManager.healthUI.activeSelf || uiManager.scoreUI.activeSelf) return;
        uiManager.healthUI.SetActive(true);
        uiManager.scoreUI.SetActive(true);
        uiManager.btnBackToMenuUI.SetActive(true);
        uiManager.btnRestart.SetActive(true);
    }
}
