using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    public bool canAttack;
    public Collider2D col;
    [SerializeField] private string tag;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tag))
        {
            canAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tag))
        {
            canAttack = false;
        }
    }
}
