using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private AudioSource[] audioEffect;
        [SerializeField] private Slider sliderEffect;
        [SerializeField] private GameObject btnSetting;
        private PlayerAudio playerAudio;
        [SerializeField] public GameObject healthUI;
        [SerializeField] public GameObject scoreUI;
        [SerializeField] private PlayerData player;

        private void Awake()
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

            playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
            sliderMusic.value = audioMusic.volume;
            sliderEffect.value = audioEffect[1].volume;
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Escape)) return;
            Time.timeScale = 0f;
            settingUI.SetActive(true);
        }

        public void DisableUISetting(bool isShow)
        {
            isShow = !isShow;
            Time.timeScale = 1f;
            settingUI.SetActive(isShow);
            btnSetting.SetActive(!isShow);
        }

        public void ShowVolumeUI(bool isShow)
        {
            isShow = !isShow;
            volumeUI.SetActive(isShow);
        }

        public void ChangeVolumeMusic()
        {
            audioMusic.volume = sliderMusic.value;
        }

        public void ChangeVolumeEffect()
        {
            foreach (var source in audioEffect)
            {
                source.volume = sliderEffect.value;
            }
        }

        public void BackToMenu()
        {
            btnSetting.SetActive(true);
            settingUI.SetActive(false);
            healthUI.SetActive(false);
            scoreUI.SetActive(false);
            SceneManager.LoadSceneAsync(0);
        }

        public void RestartLevel()
        {
            btnSetting.SetActive(true);
            settingUI.SetActive(false);
            healthUI.SetActive(false);
            scoreUI.SetActive(false);
            player.currentScenes = 0;
            SceneManager.LoadSceneAsync(0);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ButtonHover()
        {
            playerAudio.Plays_15("Hover_Effect");
        }
    }
}