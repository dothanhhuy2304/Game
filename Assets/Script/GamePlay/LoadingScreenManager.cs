using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private GameObject uILoading;
    private AsyncOperation loadOperation;
    [SerializeField] private Slider slider;

    public void LoadingScreen(int i)
    {
        StartCoroutine(nameof(LoadAsyncScene), i);
    }

    public int LoadCurrentScreen()
    {
        return player.playerDataObj.currentScenes;
    }

    private void Update()
    {
        if (!slider.IsActive()) return;
        slider.value = loadOperation.progress;
    }

    public int RestartLevel()
    {
        player.playerDataObj.currentScenes = 0;
        return player.playerDataObj.currentScenes;
    }

    public int NextScreen(int i)
    {
        player.playerDataObj.currentScenes = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + i;
        return player.playerDataObj.currentScenes;
    }

    private IEnumerator LoadAsyncScene(int index)
    {
        loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
        uILoading.SetActive(true);
        loadOperation.allowSceneActivation = false;
        while (!loadOperation.isDone)
        {
            loadOperation.allowSceneActivation = true;
            yield return new WaitForSeconds(1f);
            uILoading.SetActive(false);
        }
    }
}