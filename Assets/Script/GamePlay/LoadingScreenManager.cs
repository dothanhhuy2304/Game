using System.Collections;
using System.Linq;
using Photon.Pun;
using Script.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.GamePlay
{
    public class LoadingScreenManager : MonoBehaviourPun
    {
        public static LoadingScreenManager Instance;
        [SerializeField] private GameObject uiLoading;
        [SerializeField] private Image fillLoading;
        [SerializeField] private PhotonView pv;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public int LoadCurrentScreen()
        {
            return HuyManager.Instance.currentScreen;
        }

        public int RestartLevel()
        {
            DataService.GetConnection()
                .Execute($"update GameData set levelId = '{0}' where PlayerId = '{HuyManager.Instance.userId}'");
            return HuyManager.Instance.currentScreen = 0;
        }

        public int NextScreen()
        {
            HuyManager.Instance.currentScreen = SceneManager.GetActiveScene().buildIndex + 1;
            DataService.GetConnection()
                .Execute(
                    $"update GameData set levelId = '{HuyManager.Instance.currentScreen}' where PlayerId = '{HuyManager.Instance.userId}'");
            return HuyManager.Instance.currentScreen;
        }

        public void FadeLoadingScene(int sceneIndex,bool photonNetwork = true)
        {
            if (photonNetwork)
            {
                //StartCoroutine(LoadAsync(sceneIndex));
                pv.RPC(nameof(LoadScene), RpcTarget.AllBuffered, sceneIndex);
            }
            else
            {
                LoadOfflineScene(sceneIndex);
            }
        }

        [PunRPC]
        private void LoadScene(int index)
        {
            StartCoroutine(LoadAsync(index));
        }

        private IEnumerator LoadAsync(int sceneIndex)
        {
            uiLoading.SetActive(true);
            fillLoading.fillAmount = 0f;
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.LoadLevel(sceneIndex);
                while (PhotonNetwork.LevelLoadingProgress < 1)
                {
                    fillLoading.fillAmount = PhotonNetwork.LevelLoadingProgress * 100;
                    yield return new WaitForEndOfFrame();
                }

                Debug.LogError(PhotonNetwork.PlayerList.Any(t => t.HasRejoined));
                Debug.LogError(PhotonNetwork.PlayerList.Any(t => t.IsInactive));
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                SceneManager.LoadScene(sceneIndex);
            }

            uiLoading.SetActive(false);
        }

         private void LoadOfflineScene(int sceneIndex)
         {
             StartCoroutine(IeFadeLoadingScreen(sceneIndex));
             //StartCoroutine(IeFadeIn(sceneIndex));
         }
        
        
         private IEnumerator IeFadeIn(int scene)
         {
             uiLoading.SetActive(true);
             fillLoading.fillAmount = 0;
             AsyncOperation async = SceneManager.LoadSceneAsync(scene);
             if (async != null)
             {
                 while (!async.isDone)
                 {
                     fillLoading.fillAmount = async.progress / 0.9f;
                     yield return null;
                 }
             }
             else
             {
                 float t = 0f;
                 float time = 5f;
                 while (t < time)
                 {
                     t += Time.deltaTime;
                     float percentage = t / time;
                     percentage = percentage > 1 ? 1 : percentage;
                     fillLoading.fillAmount = percentage / 2;
                     yield return null;
                 }
             }
        
             //yield return new WaitForSeconds(0.5f);
             yield return new WaitWhile(() => async != null && !async.isDone);
             uiLoading.SetActive(false);
         }
         
         private IEnumerator IeFadeLoadingScreen(int scene)
         {
             uiLoading.SetActive(true);
             fillLoading.fillAmount = 0;
             AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
             while (!asyncOperation.isDone)
             {
                 fillLoading.fillAmount = asyncOperation.progress / 0.9f;
                 yield return null;
             }
        
             yield return new WaitWhile(() => !asyncOperation.isDone);
             uiLoading.SetActive(false);
         }
    }
}