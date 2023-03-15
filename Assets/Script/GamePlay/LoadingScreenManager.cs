using System.Collections;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : FastSingleton<LoadingScreenManager>
{
    [SerializeField] private GameObject uiLoading;
    [SerializeField] private Image fillLoading;
    private AsyncOperation async;

    public int LoadCurrentScreen()
    {
        return UserPref.currentScreen;
    }

    public int RestartLevel()
    {
        return UserPref.currentScreen = 0;
    }

    public int NextScreen(int i)
    {
        DataService.GetConnection().Execute($"update GameData set levelId = '{UserPref.saveScreenPass}' where PlayerId = '{UserPref.userId}'");
        return UserPref.currentScreen = SceneManager.GetActiveScene().buildIndex + i;
    }

    public void FadeLoadingScene(int sceneIndex)
    {
        uiLoading.SetActive(true);
        fillLoading.fillAmount = 0f;
        LoadScene(sceneIndex);

    }

    private void LoadScene(int sceneIndex)
    {
        //StartCoroutine(IeFadeIn());
        StartCoroutine(IeFadeLoadingScreen(sceneIndex));
    }

    #region OldVersion
    private IEnumerator IeFadeIn(int scene)
    {
        async = SceneManager.LoadSceneAsync(scene);
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

        yield return new WaitForSeconds(0.5f);
        uiLoading.SetActive(false);
        yield return null;
    }
    #endregion
    
    private IEnumerator IeFadeLoadingScreen(int scene)
    {
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