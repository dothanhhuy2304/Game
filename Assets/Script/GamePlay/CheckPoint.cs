using UnityEngine;

namespace Script.GamePlay
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private GameObject uiGuide;
        private LoadingScreenManager loadingScreenManager;

        private void Start()
        {
            loadingScreenManager = LoadingScreenManager.instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                uiGuide.SetActive(true);
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKey(KeyCode.F))
                {
                    loadingScreenManager.FadeLoadingScene(loadingScreenManager.NextScreen());
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                uiGuide.SetActive(false);
            }
        }
    }
}