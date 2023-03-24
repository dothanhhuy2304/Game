using Script.Player;
using UnityEngine;

namespace Script.GamePlay
{
    public class LoadCharacter : MonoBehaviour
    {
        [SerializeField] private GameObject[] characters;

        private void Awake()
        {
            if (!GameManager.instance)
            {
                Instantiate(Resources.Load<GameObject>("GameManager"));
            }

            characters[UserPref.characterSelected].SetActive(true);

            AudioManager.instance.Plays_Music("Music_Game");

            if (!UiManager.instance.scoreUI.activeSelf)
            {
                UiManager.instance.scoreUI.SetActive(true);
                UiManager.instance.btnBackToMenuUI.gameObject.SetActive(true);
                UiManager.instance.btnRestart.gameObject.SetActive(true);
            }
        }
    }
}