using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.GamePlay
{
    public class LoadCharacter : MonoBehaviour
    {
        [SerializeField] private GameObject[] characters;

        private void Awake()
        {
            if (GameManager.instance == null)
            {
                Instantiate(Resources.Load<GameObject>("GameManager"));
            }

            characters[HuyManager.Instance.characterSelected].SetActive(true);

            AudioManager.instance.Plays_Music("Music_Game");

            if (!UIManager.instance.scoreUi.activeSelf)
            {
                UIManager.instance.scoreUi.SetActive(true);
                UIManager.instance.btnBackToMenu.gameObject.SetActive(true);
                UIManager.instance.btnRestart.gameObject.SetActive(true);
            }
        }
    }
}