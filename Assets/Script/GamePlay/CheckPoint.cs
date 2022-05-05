using UnityEngine;

namespace Game.GamePlay
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private GameObject uiGuide;
        private LoadingScreenManager loadingScreenManager;

        private void Awake()
        {
            loadingScreenManager = FindObjectOfType<LoadingScreenManager>().GetComponent<LoadingScreenManager>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            uiGuide.SetActive(true);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (Input.GetKey(KeyCode.F))
            {
                loadingScreenManager.LoadingScreen(loadingScreenManager.NextScreen(1));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            uiGuide.SetActive(false);
        }
    }
}