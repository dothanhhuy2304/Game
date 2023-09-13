using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using Script.Core;

namespace Script.Player
{
    public class Weapon : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CharacterController2D player;
        [HideInInspector] [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] private Vector2 offset;
        private float _timeAttack;
        [SerializeField] private float resetTimeAttack;
        private int _tempIndex;

        private void Awake()
        {
            projectiles = FindObjectOfType<BulletController>().bulletPlayer;
        }

        private void LateUpdate()
        {
            if (!player.pv.IsMine)
            {
                return;
            }

            HuyManager.Instance.SetUpTime(ref _timeAttack);
            if (player.playerHealth.isDeath || player.playerHealth.isHurt || EventSystem.current.IsPointerOverGameObject())
                return;
            if (_timeAttack <= 0 && Input.GetMouseButtonDown(0))
            {
                player.pv.RPC(nameof(BulletAttack), RpcTarget.AllBuffered);
                _timeAttack = resetTimeAttack;
            }
        }

        [PunRPC]
        private void BulletAttack()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(offset);
            projectiles[index].transform.rotation = transform.rotation;
            projectiles[index].Shoot(transform);
            AudioManager.instance.Play("Player_Bullet_Shoot");
        }


        private int FindBullet()
        {
            if (_tempIndex >= projectiles.Count - 1)
            {
                return _tempIndex = 0;
            }

            _tempIndex++;
            if (projectiles[_tempIndex].gameObject.activeSelf)
            {
                FindBullet();
            }
            else
            {
                return _tempIndex;
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