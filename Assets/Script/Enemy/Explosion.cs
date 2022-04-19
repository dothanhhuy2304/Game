using UnityEngine;

public class Explosion : MonoBehaviour
{
    private bool isAttack = true;

    private void OnEnable()
    {
        isAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAttack) return;
        if (!other.CompareTag("Player")) return;
        isAttack = false;
        other.GetComponent<PlayerHealth>().GetDamage(20f);
        StartCoroutine(nameof(WaitingHide), 0.7f);
    }

    private System.Collections.IEnumerator WaitingHide(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
