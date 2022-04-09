using System.Collections;
using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Data player;
    [SerializeField] private GameObject uILoading;

    private IEnumerator WaitingLoading(float delay)
    {
        yield return new WaitForSeconds(0);
        uILoading.SetActive(true);
        yield return new WaitForSeconds(delay);
        uILoading.SetActive(false);
    }

    public void LoadingScreen()
    {
        StartCoroutine(nameof(WaitingLoading), 2f);
        StartCoroutine(nameof(LoadAsyncScene));
    }

    private IEnumerator LoadAsyncScene()
    {
        player.currentScenes = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(player.currentScenes);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            operation.allowSceneActivation = true;
            yield return null;
        }

    }
}
