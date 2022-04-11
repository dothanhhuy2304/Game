using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private GameObject uILoading;
    private AsyncOperation loadOperation;

    public void LoadingScreen()
    {
        //StartCoroutine(nameof(WaitingLoading), 3f);
        player.currentScenes = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(player.currentScenes);
        StartCoroutine(nameof(LoadAsyncScene));
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