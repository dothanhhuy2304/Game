using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay
{
    public class UIManager : FastSingleton<UIManager>
    {
        [Header("UI Setting")]
        [SerializeField] private GameObject settingUI;
        [SerializeField] private Button btnShowAndHiddenUI;
        [Header("UI Volume")]
        [SerializeField] private Button btnShowVolumeUI;
        [SerializeField] private AudioSource audioMusic;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderEffect;
        public Button btnHiddenUIVolume;
        //public GameObject healthUI;
        public GameObject scoreUI;
        public GameObject uiVolume;
        public Button btnBackToMenuUI;
        public Button btnRestart;
        private LoadingScreenManager loadingScreenManager;
        private bool isShowUISetting;
        private bool isShowUIVolume;

        private void Start()
        {
            //DontDestroyOnLoad(this);
            loadingScreenManager = LoadingScreenManager.instance;
            btnShowAndHiddenUI.onClick.AddListener(() => { ShowAndHiddenUiSetting(ref isShowUISetting); });
            btnBackToMenuUI.onClick.AddListener(BackToMenu);
            btnRestart.onClick.AddListener(RestartLevel);
            btnShowVolumeUI.onClick.AddListener(() => { ShowVolumeUi(ref isShowUIVolume); });
            sliderMusic.onValueChanged.AddListener(ChangeVolumeMusic);
            sliderEffect.onValueChanged.AddListener(ChangeVolumeEffect);
            btnHiddenUIVolume.onClick.AddListener(HiddenVolumeUi);
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

            btnBackToMenuUI.gameObject.SetActive(false);
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

        private void ShowVolumeUi(ref bool isShow)
        {
            isShow = !isShow;
            btnShowAndHiddenUI.gameObject.SetActive(false);
            uiVolume.gameObject.SetActive(isShow);
        }

        private void HiddenVolumeUi()
        {
            isShowUIVolume = false;
            btnShowAndHiddenUI.gameObject.SetActive(true);
            uiVolume.gameObject.SetActive(false);
        }

        private void ChangeVolumeMusic(float sliderValue)
        {
            audioMusic.volume = sliderValue;
            DataService.PlayerSetting playerSetting = new DataService.PlayerSetting();
            var player = DataService.GetConnection().Table<DataService.PlayerSetting>().FirstOrDefault();
            playerSetting.PlayerId = player.PlayerId;
            playerSetting.soundMusic = sliderValue;
            playerSetting.soundEffect = player.soundEffect;
            DataService.GetConnection().Table<DataService.PlayerSetting>().Connection.Update(playerSetting);
        }

        private void ChangeVolumeEffect(float sliderValue)
        {
            foreach (var source in AudioManager.instance.sounds)
            {
                source.audioFX.volume = sliderValue;
            }

            DataService.PlayerSetting playerSetting = new DataService.PlayerSetting();
            var player = DataService.GetConnection().Table<DataService.PlayerSetting>().FirstOrDefault();
            playerSetting.PlayerId = player.PlayerId;
            playerSetting.soundMusic = player.soundMusic;
            playerSetting.soundEffect = sliderValue;
            DataService.GetConnection().Table<DataService.PlayerSetting>().Connection.Update(playerSetting);
        }

        private void BackToMenu()
        {
            isShowUISetting = false;
            isShowUISetting = false;
            Time.timeScale = 1f;
            settingUI.SetActive(false);
            scoreUI.SetActive(false);
            btnBackToMenuUI.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
            AudioManager.instance.Plays_Music("Music_Menu");
            loadingScreenManager.FadeLoadingScene(0);
        }

        private void RestartLevel()
        {
            isShowUISetting = false;
            isShowUISetting = false;
            Time.timeScale = 1f;
            settingUI.SetActive(false);
            scoreUI.SetActive(false);
            btnBackToMenuUI.gameObject.SetActive(false);
            btnRestart.gameObject.SetActive(false);
            AudioManager.instance.Plays_Music("Music_Menu");
            loadingScreenManager.FadeLoadingScene(LoadingScreenManager.RestartLevel());
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