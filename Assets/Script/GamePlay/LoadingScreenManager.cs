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
        UserPref.currentScreen = 0;
        return UserPref.currentScreen;
    }

    public int NextScreen(int i)
    {
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
        async = SceneManager.LoadSceneAsync(sceneIndex);
        StartCoroutine(IeFadeIn());
    }

    private IEnumerator IeFadeIn()
    {
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
}