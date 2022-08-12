using System.Collections.Generic;
using Game.GamePlay;
using Game.Player;
using UnityEngine;

namespace Game.Enemy
{
    public class EnemyManager : FastSingleton<EnemyManager>
    {
        public List<FireProjectile> projectiles;
        public List<ProjectileArc> projectileArcs;
        private CharacterController2D playerCharacter;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            playerCharacter = CharacterController2D.instance;
        }


        public void AttackBullet(Vector3 offsetAttack)
        {
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack;
            projectiles[FindBullet(projectiles)].transform.rotation = transform.rotation;
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        public void AttackBulletDirection(Vector3 offsetAttack)
        {
            Vector2 directionToPlayer = (playerCharacter.transform.position - transform.position).normalized;
            projectiles[FindBullet(projectiles)].transform.position = offsetAttack;
            projectiles[FindBullet(projectiles)].transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg);
            projectiles[FindBullet(projectiles)].Shoot();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        public void AttackBulletArc(Vector3 offsetAttack)
        {
            projectileArcs[FindBullet(projectileArcs)].transform.rotation = Quaternion.identity;
            projectileArcs[FindBullet(projectileArcs)].transform.position = offsetAttack;
            projectileArcs[FindBullet(projectileArcs)].SetActives();
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        private static int FindBullet(List<FireProjectile> projectile)
        {
            for (var i = 0; i < projectile.Count; i++)
            {
                if (!projectile[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }

        private static int FindBullet(List<ProjectileArc> projectileArc)
        {
            for (var i = 0; i < projectileArc.Count; i++)
            {
                if (!projectileArc[i].gameObject.activeInHierarchy)
                    return i;
            }

            return 0;
        }

    }
}