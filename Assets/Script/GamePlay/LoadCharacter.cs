using Game.GamePlay;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class LoadCharacter : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private GameObject[] characters;

    private void Awake()
    {
        characters[playerData.characterSelection].SetActive(true);
        var playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
        Debug.Assert(playerAudio != null, nameof(playerAudio) + " != null");
        playerAudio.Plays_Music("Music_Game");
        if (UIManager.Instance.healthUI.activeSelf || UIManager.Instance.scoreUI.activeSelf) return;
        UIManager.Instance.healthUI.SetActive(true);
        UIManager.Instance.scoreUI.SetActive(true);
        UIManager.Instance.btnBackToMenuUI.SetActive(true);
        UIManager.Instance.btnRestart.SetActive(true);
    }
}
