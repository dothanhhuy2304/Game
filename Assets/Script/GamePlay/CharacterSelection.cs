using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay
{
    public class CharacterSelection : MonoBehaviour
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private GameObject[] characters;
        private int currentCharacter;
        [SerializeField] private Button btnNext, btnPreview;
        private LoadingScreenManager loadingScreenManager;

        private void Start()
        {
            loadingScreenManager = LoadingScreenManager.instance;
            for (var i = 0; i < characters.Length; i++)
            {
                characters[i].SetActive(i == 0);
            }

            UpdateButton();
        }

        public void ChangeCharacter(int index)
        {
            currentCharacter += index;
            foreach (var t in characters)
            {
                t.SetActive(false);
            }

            characters[currentCharacter].SetActive(true);
            UpdateButton();
        }

        private void UpdateButton()
        {
            btnNext.interactable = currentCharacter != characters.Length - 1;
            btnPreview.interactable = currentCharacter != 0;
        }

        public void LoadCharacter()
        {
            playerData.playerDataObj.characterSelection = currentCharacter;
            loadingScreenManager.FadeLoadingScene(playerData.playerDataObj.currentScenes == 0
                ? loadingScreenManager.NextScreen(1)
                : LoadingScreenManager.LoadCurrentScreen());
        }
    }
}