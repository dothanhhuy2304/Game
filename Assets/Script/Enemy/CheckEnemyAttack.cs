using UnityEngine;

namespace Game.Enemy
{
    public class CheckEnemyAttack : MonoBehaviour
    {

        [HideInInspector] public bool canAttack;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canAttack = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canAttack = false;
            }
        }
    }
}