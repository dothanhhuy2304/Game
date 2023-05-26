using System.Linq;
using Script.Core;
using UnityEngine;
using UnityEngine.UI;
using Script.Player;

namespace Script.GamePlay
{
    public class UIManager : FastSingleton<UIManager>
    {
        [Header("UI Setting")] 
        [SerializeField] private GameObject settingUi;
        [SerializeField] private Button btnShowAndHiddenUi;
        [Header("UI Volume")] 
        [SerializeField] private AudioSource audioMusic;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderEffect;

        //public GameObject healthUI;
        public GameObject scoreUi;
        public GameObject uiVolume;
        public Button btnBackToMenu;
        public Button btnRestart;
        private LoadingScreenManager loadingScreenManager;
        private bool isShowUiSetting;

        private void Start()
        {
            loadingScreenManager = LoadingScreenManager.instance;
            btnShowAndHiddenUi.onClick.AddListener(() => { ShowAndHiddenUiSetting(ref isShowUiSetting); });
            sliderMusic.onValueChanged.AddListener(ChangeVolumeMusic);
            sliderEffect.onValueChanged.AddListener(ChangeVolumeEffect);
            if (string.IsNullOrEmpty(HuyManager.Instance.userId))
            {
                sliderMusic.value = 1f;
                sliderEffect.value = 1f;
                audioMusic.volume = audioMusic.volume;
                return;
            }

            var playerSetting = DataService.GetConnection().Table<DataService.PlayerSetting>().FirstOrDefault();

            if (playerSetting.PlayerId.Equals(HuyManager.Instance.userId))
            {
                sliderMusic.value = playerSetting.soundMusic;
                sliderEffect.value = playerSetting.soundEffect;
                audioMusic.volume = playerSetting.soundMusic;
                foreach (var source in AudioManager.instance.sounds)
                {
                    source.audioFX.volume = playerSetting.soundEffect;
                }
            }

            btnBackToMenu.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
        }


        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Time.timeScale = 0f;
                settingUi.SetActive(true);
            }
        }

        private void ChangeVolumeMusic(float sliderValue)
        {
            audioMusic.volume = sliderValue;
            HuyManager.Instance.ChangeSettingSoundMusic(sliderValue);
        }

        private void ChangeVolumeEffect(float sliderValue)
        {
            foreach (var source in AudioManager.instance.sounds)
            {
                source.audioFX.volume = sliderValue;
            }

            HuyManager.Instance.ChangeSettingSoundEffect(sliderValue);
        }

        private void ShowAndHiddenUiSetting(ref bool isShow)
        {
            isShow = !isShow;
            settingUi.SetActive(isShow);
            Time.timeScale = isShow ? 0f : 1f;
        }

        public void ShowVolumeUi()
        {
            btnShowAndHiddenUi.gameObject.SetActive(false);
            uiVolume.gameObject.SetActive(true);
        }

        public void HiddenVolumeUi()
        {
            btnShowAndHiddenUi.gameObject.SetActive(true);
            uiVolume.gameObject.SetActive(false);
        }

        public void BackToMenu()
        {
            isShowUiSetting = false;
            Time.timeScale = 1f;
            settingUi.SetActive(false);
            scoreUi.SetActive(false);
            btnBackToMenu.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
            AudioManager.instance.Plays_Music("Music_Menu");
            loadingScreenManager.FadeLoadingScene(0);
        }

        public void RestartLevel()
        {
            isShowUiSetting = false;
            Time.timeScale = 1f;
            settingUi.SetActive(false);
            scoreUi.SetActive(false);
            btnBackToMenu.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
            AudioManager.instance.Plays_Music("Music_Menu");
            loadingScreenManager.FadeLoadingScene(loadingScreenManager.RestartLevel());
        }

        public void ExitGame()
        {
            Time.timeScale = 1f;
            Application.Quit();
        }

        public void PopupCreateAccount()
        {
            Instantiate(Resources.Load<GameObject>("PopupRegister"));
        }

        public void ButtonHover()
        {
            if (AudioManager.instance != null) AudioManager.instance.Play("Hover_Effect");
        }
    }
}