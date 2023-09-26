using System.Collections;
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
        //private AsyncOperation async;

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

        [PunRPC]
        public void FadeLoadingScene(int sceneIndex)
        {
            //LoadScene(sceneIndex);
            StartCoroutine(LoadAsync(sceneIndex));
        }


        private IEnumerator LoadAsync(int sceneIndex)
        {
            uiLoading.SetActive(true);
            fillLoading.fillAmount = 0f;
            PhotonNetwork.LoadLevel(sceneIndex);
            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                fillLoading.fillAmount = PhotonNetwork.LevelLoadingProgress * 100;
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(0.5f);
            uiLoading.SetActive(false);
        }

        // private void LoadScene(int sceneIndex)
        // {
        //     StartCoroutine(IeFadeIn(sceneIndex));
        //     //StartCoroutine(IeFadeLoadingScreen(sceneIndex));
        // }
        //
        // #region OldVersion
        //
        // private IEnumerator IeFadeIn(int scene)
        // {
        //     async = SceneManager.LoadSceneAsync(scene);
        //     if (async != null)
        //     {
        //         while (!async.isDone)
        //         {
        //             fillLoading.fillAmount = async.progress / 0.9f;
        //             yield return null;
        //         }
        //     }
        //     else
        //     {
        //         float t = 0f;
        //         float time = 5f;
        //         while (t < time)
        //         {
        //             t += Time.deltaTime;
        //             float percentage = t / time;
        //             percentage = percentage > 1 ? 1 : percentage;
        //             fillLoading.fillAmount = percentage / 2;
        //             yield return null;
        //         }
        //     }
        //
        //     //yield return new WaitForSeconds(0.5f);
        //     yield return new WaitWhile(() => !async.isDone);
        //     uiLoading.SetActive(false);
        //     yield return null;
        // }
        //
        //#endregion
        //
        // private IEnumerator IeFadeLoadingScreen(int scene)
        // {
        //     AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
        //     while (!asyncOperation.isDone)
        //     {
        //         fillLoading.fillAmount = asyncOperation.progress / 0.9f;
        //         yield return null;
        //     }
        //
        //     yield return new WaitWhile(() => !asyncOperation.isDone);
        //     uiLoading.SetActive(false);
        // }
    }
}