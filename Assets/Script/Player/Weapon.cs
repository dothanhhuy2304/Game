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
        [SerializeField] private float resetTimeAttack = 0.5f;

        private void LateUpdate()
        {
            if (HuyManager.PlayerIsDeath() || EventSystem.current.IsPointerOverGameObject()) return;
            HuyManager.SetTimeAttack(ref timeAttack);
            if (timeAttack <= 0)
            {
                if (!HuyManager.GetPlayerIsHurt())
                {
                    if (Input.GetMouseButtonDown(0)||Input.GetKeyDown(KeyCode.L))
                    {
                        BulletAttack();
                        timeAttack = resetTimeAttack;
                    }
                }
            }
        }

        private void BulletAttack()
        {
            projectiles[FindBullet()].transform.position = transform.TransformPoint(offset);
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].Shoot(transform);
            AudioManager.instance.Play("Player_Bullet_Shoot");
        }

        private int FindBullet()
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

    }
}