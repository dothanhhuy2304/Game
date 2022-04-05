using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private GameObject uiGuide;
    [SerializeField] private Data player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        uiGuide.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!Input.GetKey(KeyCode.F)) return;
        player.currentScenes = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(player.currentScenes);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        uiGuide.SetActive(false);
    }
}