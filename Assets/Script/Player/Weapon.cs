using Game.Core;
using Game.GamePlay;
using UnityEngine;

namespace Game.Player
{
    public class Weapon : BaseObject
    {
        [SerializeField] private FireProjectile[] projectiles;
        [SerializeField] private Vector2 offset;
        [SerializeField] private PlayerHealth playerHealth;
        private CharacterController2D players;
        private float timeAttack;
        [SerializeField] private float resetTimeAttack = 0.5f;
        private PlayerAudio playerAudio;

        protected override void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
            players = GetComponent<CharacterController2D>();
            playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
            timeAttack = 0f;
        }

        private void LateUpdate()
        {
            SetTimeAttack(ref timeAttack);
            if (timeAttack != 0) return;
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() ||
                playerHealth.PlayerIsDeath()) return;
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
            //Instantiate(fireObj, transform.TransformPoint(offset), transform.rotation);
            projectiles[FindBullet()].transform.position = transform.TransformPoint(offset);
            projectiles[FindBullet()].transform.rotation = transform.rotation;
            projectiles[FindBullet()].SetActives();
            playerAudio.Play("Player_Bullet_Shoot");
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