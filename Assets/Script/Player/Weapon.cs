using System.Collections.Generic;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] private Vector2 offset;
        private float timeAttack;
        [SerializeField] private float resetTimeAttack;

        private void LateUpdate()
        {
            HuyManager.SetTimeAttack(ref timeAttack);
            if (HuyManager.PlayerIsDeath() || HuyManager.GetPlayerIsHurt() || EventSystem.current.IsPointerOverGameObject()) return;
            if (timeAttack <= 0)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.L))
                {
                    BulletAttack();
                    timeAttack = resetTimeAttack;
                }
            }
        }

        private void BulletAttack()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(offset);
            projectiles[index].transform.rotation = transform.rotation;
            projectiles[index].Shoot(transform);
            AudioManager.instance.Play("Player_Bullet_Shoot");
        }

        private int tempIndex;

        private int FindBullet()
        {
            if (tempIndex >= projectiles.Count - 1)
            {
                return tempIndex = 0;
            }

            tempIndex++;
            if (!projectiles[tempIndex].gameObject.activeSelf)
            {
                return tempIndex;
            }

            return 0;
        }

        #region Old Version

        private int FindBullets()
        {
            for (var i = 0; i < projectiles.Count; i++)
            {
                if (!projectiles[i].gameObject.activeSelf)
                {
                    return i;
                }
            }

            return 0;
        }

        #endregion
    }
}