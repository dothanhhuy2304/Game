using UnityEngine;

namespace Game.GamePlay
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject settingUI;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (!Input.GetKey(KeyCode.Escape)) return;
            Time.timeScale = 0f;
            settingUI.SetActive(true);
        }

        public void DisableUISetting()
        {
            Time.timeScale = 1f;
            settingUI.SetActive(false);
        }
    }
}