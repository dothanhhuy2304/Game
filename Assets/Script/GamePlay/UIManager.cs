using UnityEngine;
using UnityEngine.UI;

namespace Game.GamePlay
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }
        [SerializeField] private GameObject settingUI;
        [SerializeField] private GameObject volumeUI;
        [SerializeField] private AudioSource audioMusic;
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderEffect;
        [SerializeField] private GameObject btnSetting;
        public GameObject healthUI;
        public GameObject scoreUI;
        public GameObject btnBackToMenuUI;
        public GameObject btnRestart;
        [SerializeField] private PlayerData playerData;
        private LoadingScreenManager loadingScreenManager;

        private void Start()
        {
            DontDestroyOnLoad(this);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            loadingScreenManager = FindObjectOfType<LoadingScreenManager>().GetComponent<LoadingScreenManager>();
            if (!playerData.saveAudio)
            {
                playerData.soundMusic = audioMusic.volume;
                playerData.soundEffect = PlayerAudio.Instance.sounds[0].audioFX.volume;
                playerData.saveAudio = true;
            }

            sliderMusic.value = playerData.soundMusic;
            sliderEffect.value = playerData.soundEffect;
            audioMusic.volume = playerData.soundMusic;
            foreach (var source in PlayerAudio.Instance.sounds)
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
            foreach (var source in PlayerAudio.Instance.sounds)
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
            PlayerAudio.Instance.Plays_Music("Music_Menu");
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
            PlayerAudio.Instance.Plays_Music("Music_Menu");
            loadingScreenManager.LoadingScreen(loadingScreenManager.RestartLevel());
        }

        public void ExitGame()
        {
            Time.timeScale = 1f;
            Application.Quit();
        }

        public void ButtonHover()
        {
            PlayerAudio.Instance.Play("Hover_Effect");
            //playerAudio.Plays_15("Hover_Effect");
        }
    }
}