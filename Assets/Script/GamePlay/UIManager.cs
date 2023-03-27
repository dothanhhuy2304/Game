using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Script.Player;

namespace Script.GamePlay
{
    public class UiManager : FastSingleton<UiManager>
    {
        [Header("UI Setting")]
        [SerializeField] private GameObject settingUI;
        [SerializeField] private Button btnShowAndHiddenUI;
        [Header("UI Volume")]
        [SerializeField] private Button btnShowVolume;
        [SerializeField] private AudioSource audioMusic;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderEffect;
        public Button btnHiddenUiVolume;
        //public GameObject healthUI;
        public GameObject scoreUI;
        public GameObject uiVolume;
        public Button btnBackToMenu;
        public Button btnRestart;
        private LoadingScreenManager loadingScreenManager;
        private bool isShowUISetting;
        private bool isShowUIVolume;

        private void Start()
        {
            loadingScreenManager = LoadingScreenManager.instance;
            btnShowAndHiddenUI.onClick.AddListener(() => { ShowAndHiddenUiSetting(ref isShowUISetting); });
            btnBackToMenu.onClick.AddListener(BackToMenu);
            btnRestart.onClick.AddListener(RestartLevel);
            //btnShowVolume.onClick.AddListener(() => { ShowVolumeUi(ref isShowUIVolume); });
            sliderMusic.onValueChanged.AddListener(ChangeVolumeMusic);
            sliderEffect.onValueChanged.AddListener(ChangeVolumeEffect);
            //btnHiddenUiVolume.onClick.AddListener(HiddenVolumeUi);
            if (string.IsNullOrEmpty(UserPref.userId))
            {
                sliderMusic.value = 1f;
                sliderEffect.value = 1f;
                audioMusic.volume = audioMusic.volume;
                return;
            }

            var playerSetting = DataService.GetConnection().Table<DataService.PlayerSetting>().ToList();

            foreach (var player in playerSetting)
            {
                if (player.PlayerId.Equals(UserPref.userId))
                {
                    sliderMusic.value = player.soundMusic;
                    sliderEffect.value = player.soundEffect;
                    audioMusic.volume = player.soundMusic;
                    foreach (var source in AudioManager.instance.sounds)
                    {
                        source.audioFX.volume = player.soundEffect;
                    }
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
                settingUI.SetActive(true);
            }
        }

        private void ShowAndHiddenUiSetting(ref bool isShow)
        {
            isShow = !isShow;
            settingUI.SetActive(isShow);
            Time.timeScale = isShow ? 0f : 1f;
        }

        public void ShowVolumeUi()
        {
            btnShowAndHiddenUI.gameObject.SetActive(false);
            uiVolume.gameObject.SetActive(true);
        }

        public void HiddenVolumeUi()
        {
            isShowUIVolume = false;
            btnShowAndHiddenUI.gameObject.SetActive(true);
            uiVolume.gameObject.SetActive(false);
        }

        private void ChangeVolumeMusic(float sliderValue)
        {
            audioMusic.volume = sliderValue;
            if (DataService.GetConnection().Table<DataService.PlayerSetting>().Any())
            {
                DataService.GetConnection().Execute($"update PlayerSetting set soundMusic = '{sliderValue}' where PlayerId = '{UserPref.userId}'");
            }
        }

        private void ChangeVolumeEffect(float sliderValue)
        {
            foreach (var source in AudioManager.instance.sounds)
            {
                source.audioFX.volume = sliderValue;
            }

            if (DataService.GetConnection().Table<DataService.PlayerSetting>().Any())
            {
                DataService.GetConnection().Execute($"update PlayerSetting set soundEffect = '{sliderValue}' where PlayerId = '{UserPref.userId}'");
            }
        }

        public void BackToMenu()
        {
            isShowUISetting = false;
            isShowUISetting = false;
            Time.timeScale = 1f;
            settingUI.SetActive(false);
            scoreUI.SetActive(false);
            btnBackToMenu.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
            AudioManager.instance.Plays_Music("Music_Menu");
            loadingScreenManager.FadeLoadingScene(0);
        }

        public void RestartLevel()
        {
            isShowUISetting = false;
            isShowUISetting = false;
            Time.timeScale = 1f;
            settingUI.SetActive(false);
            scoreUI.SetActive(false);
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
            AudioManager.instance.Play("Hover_Effect");
        }
    }
}