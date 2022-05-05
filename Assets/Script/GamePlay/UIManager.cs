using UnityEngine;

namespace Game.GamePlay
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        [SerializeField] private GameObject settingUI;
        [SerializeField] private GameObject volumeUI;
        [SerializeField] private AudioSource audioMusic;
        [SerializeField] private UnityEngine.UI.Slider sliderMusic;
        [SerializeField] private UnityEngine.UI.Slider sliderEffect;
        [SerializeField] private GameObject btnSetting;
        public GameObject healthUI;
        public GameObject scoreUI;
        public GameObject btnBackToMenuUI;
        public GameObject btnRestart;
        [SerializeField] private PlayerData playerData;
        [SerializeField] private LoadingScreenManager loadingScreenManager;
        [SerializeField] private PlayerAudio playerAudio;

        private void Start()
        {
            DontDestroyOnLoad(this);

            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            if (!playerData.saveAudio)
            {
                playerData.soundMusic = audioMusic.volume;
                playerData.soundEffect = playerAudio.sounds[0].audioFX.volume;
                playerData.saveAudio = true;
            }

            sliderMusic.value = playerData.soundMusic;
            sliderEffect.value = playerData.soundEffect;
            audioMusic.volume = playerData.soundMusic;
            foreach (var source in playerAudio.sounds)
            {
                source.audioFX.volume = playerData.soundEffect;
            }

            btnBackToMenuUI.SetActive(false);
            btnRestart.SetActive(false);
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Escape)) return;
            Time.timeScale = 0f;
            settingUI.SetActive(true);
        }

        public void OpenAndDisableUISetting(bool isShow)
        {
            isShow = !isShow;
            settingUI.SetActive(isShow);
            btnSetting.SetActive(!isShow);
            Time.timeScale = isShow ? 0f : 1f;
        }

        public void ShowVolumeUI(bool isShow)
        {
            isShow = !isShow;
            volumeUI.SetActive(isShow);
        }

        public void ChangeVolumeMusic()
        {
            var value = sliderMusic.value;
            audioMusic.volume = value;
            playerData.soundMusic = value;
        }

        public void ChangeVolumeEffect()
        {
            foreach (var source in playerAudio.sounds)
            {
                source.audioFX.volume = sliderEffect.value;
            }

            playerData.soundEffect = sliderEffect.value;
        }

        public void BackToMenu()
        {
            Time.timeScale = 1f;
            btnSetting.SetActive(true);
            settingUI.SetActive(false);
            healthUI.SetActive(false);
            scoreUI.SetActive(false);
            btnBackToMenuUI.SetActive(false);
            btnRestart.SetActive(false);
            playerAudio.Plays_Music("Music_Menu");
            loadingScreenManager.LoadingScreen(0);
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            btnSetting.SetActive(true);
            settingUI.SetActive(false);
            healthUI.SetActive(false);
            scoreUI.SetActive(false);
            btnBackToMenuUI.SetActive(false);
            btnRestart.SetActive(false);
            playerAudio.Plays_Music("Music_Menu");
            loadingScreenManager.LoadingScreen(loadingScreenManager.RestartLevel());
        }

        public void ExitGame()
        {
            Time.timeScale = 1f;
            Application.Quit();
        }

        public void ButtonHover()
        {
            playerAudio.Play("Hover_Effect");
            //playerAudio.Plays_15("Hover_Effect");
        }
    }
}