using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private GameObject uILoading;
    private AsyncOperation loadOperation;

    public void LoadingScreen(int i)
    {
        //StartCoroutine(nameof(WaitingLoading), 3f);
        loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(i);
        StartCoroutine(nameof(LoadAsyncScene));
    }

    public int LoadCurrentScreen()
    {
        return player.currentScenes;
    }

    public int ResetScreen()
    {
        player.currentScenes = 0;
        return player.currentScenes;
    }

    public int NextScreen(int i)
    {
        player.currentScenes = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + i;
        return player.currentScenes;
    }

    private IEnumerator LoadAsyncScene()
    {
        uILoading.SetActive(true);
        loadOperation.allowSceneActivation = false;
        while (!loadOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
            loadOperation.allowSceneActivation = true;
            yield return new WaitForSeconds(1f);
            uILoading.SetActive(false);
        }
    }
}