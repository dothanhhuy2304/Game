using Game.GamePlay;
using UnityEngine;
using Game.Player;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Collider2D col;

    private void OnEnable()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AudioManager.instance.Play("Boom_Explosion");
        StartCoroutine(WaitingHide(0.7f));
        if (other.CompareTag("Player"))
        {
            PlayerHealth.instance.GetDamage(20f);
        }
    }

    private System.Collections.IEnumerator WaitingHide(float delay)
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}