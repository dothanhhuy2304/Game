using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasSetting : MonoBehaviour
{
    [SerializeField] private GameObject canvasUI;
    [SerializeField] private AudioClip soundHover;
    private AudioSource audioSource;
    private bool isShow;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowAndHideMenu()
    {
        isShow = !isShow;
        this.canvasUI.SetActive(isShow);
    }

    public void HoverSound() => audioSource.PlayOneShot(soundHover);

    public void PlayGame() => SceneManager.LoadSceneAsync(1);
}
