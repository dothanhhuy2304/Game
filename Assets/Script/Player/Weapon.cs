using Game.GamePlay;
using UnityEngine;

namespace Game.Player
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private FireProjectile[] projectiles;
        [SerializeField] private Vector2 offset;
        private CharacterController2D players;
        private float timeAttack;
        [SerializeField] private float resetTimeAttack = 0.5f;

        private void Start()
        {
            players = CharacterController2D.instance;
            timeAttack = 0f;
        }

        private void LateUpdate()
        {
            HuyManager.SetTimeAttack(ref timeAttack);
            if (timeAttack != 0) return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() || HuyManager.PlayerIsDeath()) return;
            if (players.isHurt) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;
            Attacks();
        }

        public void Attacks()
        {
            if (timeAttack != 0) return;
            Bullet();
            timeAttack = resetTimeAttack;
        }

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