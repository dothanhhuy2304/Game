using System.Collections.Generic;
using Script.Core;
using Script.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Script.GamePlay
{
    public class CharacterSelection : MonoBehaviour
    {
        [SerializeField] private List<GameObject> characters;
        private int currentCharacter;
        [SerializeField] private Button btnNext, btnPreview;
        private LoadingScreenManager loadingScreenManager;

        private void Start()
        {
            loadingScreenManager = LoadingScreenManager.instance;
            for (var i = 0; i < characters.Count; i++)
            {
                characters[i].SetActive(i == 0);
            }

            SwitchCharacterIndex();
        }

        public void ChangeCharacter(int index)
        {
            currentCharacter += index;
            foreach (var t in characters)
            {
                t.SetActive(false);
            }

            characters[currentCharacter].SetActive(true);
            SwitchCharacterIndex();
        }

        private void SwitchCharacterIndex()
        {
            btnNext.interactable = currentCharacter != characters.Count - 1;
            btnPreview.interactable = currentCharacter != 0;
        }

        public void LoadCharacter()
        {
            HuyManager.Instance.characterSelected = currentCharacter;
            loadingScreenManager.FadeLoadingScene(
                HuyManager.Instance.currentScreen == 0
                    ? loadingScreenManager.NextScreen()
                    : loadingScreenManager.LoadCurrentScreen());
            //loadingScreenManager.FadeLoadingScene(gameManager.playerData.playerDataObj.currentScenes == 0 ? loadingScreenManager.NextScreen(1) : LoadingScreenManager.LoadCurrentScreen());
        }

        public void PlayEffectClick()
        {
            if (AudioManager.instance != null) AudioManager.instance.Play("Hover_Effect");
        }
    }
}