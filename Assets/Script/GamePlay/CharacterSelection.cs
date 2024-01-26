using System.Collections.Generic;
using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Script.GamePlay
{
    public class CharacterSelection : MonoBehaviourPun
    {
        [SerializeField] private List<GameObject> characters;
        private int _currentCharacter;
        [SerializeField] private Button btnNext, btnPreview;
        private LoadingScreenManager _loadingScreenManager;

        private void Start()
        {
            _loadingScreenManager = LoadingScreenManager.Instance;
            for (var i = 0; i < characters.Count; i++)
            {
                characters[i].SetActive(i == 0);
            }

            SwitchCharacterIndex();
        }

        public void ChangeCharacter(int index)
        {
            _currentCharacter += index;
            foreach (var t in characters)
            {
                t.SetActive(false);
            }

            characters[_currentCharacter].SetActive(true);
            SwitchCharacterIndex();
        }

        private void SwitchCharacterIndex()
        {
            btnNext.interactable = _currentCharacter != characters.Count - 1;
            btnPreview.interactable = _currentCharacter != 0;
        }

        public void LoadCharacter()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            photonView.RPC(nameof(RpcCharacter), RpcTarget.AllBuffered);
            _loadingScreenManager.FadeLoadingScene(
                HuyManager.Instance.currentScreen == 0
                    ? _loadingScreenManager.NextScreen()
                    : _loadingScreenManager.LoadCurrentScreen());
        }

        [PunRPC]
        private void RpcCharacter()
        {
            HuyManager.Instance.characterSelected = _currentCharacter;
        }

        public void PlayEffectClick()
        {
            if (AudioManager.instance != null) AudioManager.instance.Play("Hover_Effect");
        }
    }
}