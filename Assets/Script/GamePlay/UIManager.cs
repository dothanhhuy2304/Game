using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay
{
    public class UIManager : FastSingleton<UIManager>
    {
        [Header("UI Setting")]
        private GameManager gameManager;
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
            gameManager = GameManager.instance;
            loadingScreenManager = LoadingScreenManager.instance;
            btnShowAndHiddenUI.onClick.AddListener(delegate {ShowAndHiddenUISetting(ref isShowUISetting);});
            btnBackToMenuUI.onClick.AddListener(BackToMenu);
            btnRestart.onClick.AddListener(RestartLevel);
            btnShowVolumeUI.onClick.AddListener(delegate { ShowVolumeUI(ref isShowUIVolume); });
            sliderMusic.onValueChanged.AddListener(ChangeVolumeMusic);
            sliderEffect.onValueChanged.AddListener(ChangeVolumeEffect);
            btnHiddenUIVolume.onClick.AddListener(HiddenVolumeUI);
            if (!gameManager.playerData.playerDataObj.saveAudio)
            {
                gameManager.playerData.playerDataObj.soundMusic = audioMusic.volume;
                gameManager.playerData.playerDataObj.soundEffect = AudioManager.instance.sounds[0].audioFX.volume;
                gameManager.playerData.playerDataObj.saveAudio = true;
            }

            sliderMusic.value = gameManager.playerData.playerDataObj.soundMusic;
            sliderEffect.value = gameManager.playerData.playerDataObj.soundEffect;
            audioMusic.volume = gameManager.playerData.playerDataObj.soundMusic;
            foreach (var source in AudioManager.instance.sounds)
            {
                source.audioFX.volume = gameManager.playerData.playerDataObj.soundEffect;
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

        private void ShowAndHiddenUISetting(ref bool isShow)
        {
            isShow = !isShow;
            settingUI.SetActive(isShow);
            Time.timeScale = isShow ? 0f : 1f;
        }

        private void ShowVolumeUI(ref bool isShow)
        {
            isShow = !isShow;
            btnShowAndHiddenUI.gameObject.SetActive(false);
            uiVolume.gameObject.SetActive(isShow);
        }

        private void HiddenVolumeUI()
        {
            isShowUIVolume = false;
            btnShowAndHiddenUI.gameObject.SetActive(true);
            uiVolume.gameObject.SetActive(false);
        }

        private void ChangeVolumeMusic(float sliderValue)
        {
            audioMusic.volume = sliderValue;
            gameManager.playerData.playerDataObj.soundMusic = sliderValue;
        }

        private void ChangeVolumeEffect(float sliderValue)
        {
            gameManager.playerData.playerDataObj.soundEffect = sliderValue;
            foreach (var source in AudioManager.instance.sounds)
            {
                source.audioFX.volume = sliderValue;
            }
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

        public void ButtonHover()
        {
            AudioManager.instance.Play("Hover_Effect");
        }
    }
}