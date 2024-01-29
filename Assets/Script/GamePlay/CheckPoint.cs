using Photon.Pun;
using UnityEngine;

namespace Script.GamePlay
{
    public class CheckPoint : MonoBehaviourPun
    {
        [SerializeField] private GameObject uiGuide;
        private LoadingScreenManager _loadingScreenManager;

        private void Start()
        {
            _loadingScreenManager = LoadingScreenManager.Instance;
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
                    _loadingScreenManager.FadeLoadingScene(_loadingScreenManager.NextScreen());
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