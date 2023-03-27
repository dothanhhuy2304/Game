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

            characters[UserPref.characterSelected].SetActive(true);

            AudioManager.instance.Plays_Music("Music_Game");

            if (!UIManager.instance.scoreUI.activeSelf)
            {
                UIManager.instance.scoreUI.SetActive(true);
                UIManager.instance.btnBackToMenu.gameObject.SetActive(true);
                UIManager.instance.btnRestart.gameObject.SetActive(true);
            }
        }
    }
}