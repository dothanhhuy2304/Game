using Game.GamePlay;
using UnityEngine;

public class LoadCharacter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject[] characters;

    private void Awake()
    {
        characters[playerData.playerDataObj.characterSelection].SetActive(true);
        var uiManager = UIManager.instance;
        if (!AudioManager.instance.audioMusic.clip)
        {
            AudioManager.instance.Plays_Music("Music_Game");
        }

        if (uiManager.healthUI.activeSelf || uiManager.scoreUI.activeSelf) return;
        uiManager.healthUI.SetActive(true);
        uiManager.scoreUI.SetActive(true);
        uiManager.btnBackToMenuUI.gameObject.SetActive(true);
        uiManager.btnRestart.gameObject.SetActive(true);
    }
}
