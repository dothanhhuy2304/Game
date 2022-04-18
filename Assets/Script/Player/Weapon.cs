using System.Collections;
using Game.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Player
{
    public class Weapon : BaseObject
    {
        [SerializeField] private GameObject[] bulletHolder;
        [SerializeField] private Vector2 offset;
        [SerializeField] private PlayerHealth playerHealth;
        private PlayerAudio playerAudio;
        private CharacterController2D players;
        [SerializeField] private float timeAttack = 0.5f;
        private const float ResetTimeAttack = 0.5f;

        protected override void Start()
        {
            playerHealth = GetComponent<PlayerHealth>();
            players = GetComponent<CharacterController2D>();
            playerAudio = FindObjectOfType<PlayerAudio>()?.GetComponent<PlayerAudio>();
            timeAttack = 0f;
        }

        private void LateUpdate()
        {
            if (EventSystem.current.IsPointerOverGameObject() || playerHealth.PlayerIsDeath()) return;
            if (players.isHurt) return;
            if (SetTimeAttack(ref timeAttack) != 0) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.KeypadEnter)) return;
            StartCoroutine(nameof(Attacks));
            timeAttack = ResetTimeAttack;
        }

        private IEnumerator Attacks()
        {
            Attack();
            yield return null;
        }
        
        private void Attack()
        {
            //Instantiate(fireObj, transform.TransformPoint(offset), transform.rotation);
            bulletHolder[FindBullet()].transform.position = transform.TransformPoint(offset);
            bulletHolder[FindBullet()].transform.rotation = transform.rotation;
            bulletHolder[FindBullet()].GetComponent<FireProjectile>().SetActives();
            playerAudio.Plays_20("Player_Bullet_Shoot");
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