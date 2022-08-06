using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay
{
    public class CharacterSelection : MonoBehaviour
    {
        private GameManager gameManager;
        [SerializeField] private List<GameObject> characters;
        private int currentCharacter;
        [SerializeField] private Button btnNext, btnPreview;
        private LoadingScreenManager loadingScreenManager;

        private void Start()
        {
            gameManager = GameManager.instance;
            loadingScreenManager = LoadingScreenManager.instance;
            for (var i = 0; i < characters.Count; i++)
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
            btnNext.interactable = currentCharacter != characters.Count - 1;
            btnPreview.interactable = currentCharacter != 0;
        }

        public void LoadCharacter()
        {
            gameManager.playerData.playerDataObj.characterSelection = currentCharacter;
            loadingScreenManager.FadeLoadingScene(gameManager.playerData.playerDataObj.currentScenes == 0 ? loadingScreenManager.NextScreen(1) : LoadingScreenManager.LoadCurrentScreen());
        }
    }
}