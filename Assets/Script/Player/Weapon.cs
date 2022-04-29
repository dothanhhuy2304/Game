using System.Collections;
using Game.Core;
using Game.GamePlay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Player
{
    public class Weapon : BaseObject
    {
        [SerializeField] private GameObject[] bulletHolder;
        [SerializeField] private Vector2 offset;
        [SerializeField] private PlayerHealth playerHealth;
        private CharacterController2D players;
        [SerializeField] private float timeAttack = 0.5f;
        private const float ResetTimeAttack = 0.5f;
        protected override void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
            players = GetComponent<CharacterController2D>();
            timeAttack = 0f;
        }

        private void LateUpdate()
        {
            if (SetTimeAttack(ref timeAttack) != 0) return;
            if (EventSystem.current.IsPointerOverGameObject() || playerHealth.PlayerIsDeath()) return;
            if (players.isHurt) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;
            Attacks();
        }

        public void Attacks()
        {
            if (timeAttack != 0) return;
            StartCoroutine(nameof(Attack));
            timeAttack = ResetTimeAttack;
        }

        private IEnumerator Attack()
        {
            Bullet();
            yield return null;
        }
        
        private void Bullet()
        {
            //Instantiate(fireObj, transform.TransformPoint(offset), transform.rotation);
            bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offset);
            bulletHolder[FindBullet()].transform.rotation = transform.rotation;
            bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
            PlayerAudio.Instance.Play("Player_Bullet_Shoot");
            //playerAudio.Plays_20("Player_Bullet_Shoot");
        }

        private int FindBullet()
        {
            for (var i = 0; i < bulletHolder.Length; i++)
            {
                if (!bulletHolder[i].activeInHierarchy)
                {
                    return i;
                }
            }

            return 0;
        }

    }
}