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

    public static int LoadCurrentScreen()
    {
        return GameManager.instance.playerData.playerDataObj.currentScenes;
    }

    public static int RestartLevel()
    {
        return GameManager.instance.playerData.playerDataObj.currentScenes = 0;
    }

    public int NextScreen(int i)
    {
        return GameManager.instance.playerData.playerDataObj.currentScenes = SceneManager.GetActiveScene().buildIndex + i;
    }

    public void FadeLoadingScene(int sceneIndex)
    {
        uiLoading.SetActive(true);
        fillLoading.fillAmount = 0f;
        StartCoroutine(DelayFrameToLoadScene(sceneIndex));

    }

    private IEnumerator DelayFrameToLoadScene(int sceneIndex)
    {
        yield return null;
        async = SceneManager.LoadSceneAsync(sceneIndex);
        StartCoroutine(IEFadeIn());
    }

    private IEnumerator IEFadeIn()
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