using UnityEngine;

namespace Game.Enemy
{
    public class CarnivorousPlant : EnemyController
    {
        private void Update()
        {
            CheckUpdate();
            if (playerHealth.PlayerIsDeath()) return;
            if (enemyHealth.EnemyDeath()) return;
            if (Vector3.Distance(transform.position, player.position) > rangeAttack) return;
            Flip();
            if (currentTime != 0f) return;
            animator.SetTrigger(animationState.carnivorousPlantIsAttack);
            currentTime = maxTimeAttack;
            Attack();
        }
    }
}