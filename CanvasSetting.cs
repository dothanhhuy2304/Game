using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasSetting : MonoBehaviour
{
    [SerializeField]private GameObject canvas;
    private AudioSource audioSource;
    public AudioClip hover;
    [SerializeField] private bool isShow = false;
    private void Awake()
    {
        canvas = GameObject.Find("CanvasUI");
        audioSource = GetComponent<AudioSource>();
        canvas.SetActive(isShow);
    }
    private void ShowAndHideMenu()
    {
        isShow = !isShow;
        canvas.SetActive(isShow);
    }
    private void PlayGame()
    {
        SceneManager.LoadSceneAsync("Game1");
    }
    private void HoverSound()
    {
        audioSource.PlayOneShot(hover);
    }

}
