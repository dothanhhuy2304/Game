using Game.GamePlay;
using UnityEngine;

namespace Game.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private FireProjectile[] projectiles;
        [SerializeField] private Vector2 offset;
        private float timeAttack;
        [SerializeField] private float resetTimeAttack = 0.5f;

        // private void Start()
        // {
        //     timeAttack = 0f;
        // }

        private void LateUpdate()
        {
            if (HuyManager.PlayerIsDeath() || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            HuyManager.SetTimeAttack(ref timeAttack);
            if (timeAttack <= 0)
            {
                if (!HuyManager.GetPlayerIsHurt())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Bullet();
                        //reset time when player shoot
                        timeAttack = resetTimeAttack;
                    }
                }
            }
        }

        // private void Attacks()
        // {
        //     if (timeAttack <= 0)
        //     {
        //         Bullet();
        //         timeAttack = resetTimeAttack;
        //     }
        // }

        private void Bullet()
        {
            projectiles[FindBullet()].transform.position = transform.TransformPoint(offset);
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].SetActives();
            AudioManager.instance.Play("Player_Bullet_Shoot");
        }

        private int FindBullet()
        {
            for (var i = 0; i < projectiles.Length; i++)
            {
                if (!projectiles[i].gameObject.activeInHierarchy)
                {
                    return i;
                }
            }

            return 0;
        }

    }
}