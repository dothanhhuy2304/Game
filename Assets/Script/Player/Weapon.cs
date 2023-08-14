using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using Script.Core;

namespace Script.Player
{
    public class Weapon : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] private Vector2 offset;
        private float timeAttack;
        [SerializeField] private float resetTimeAttack;
        private int _tempIndex;

        private void Awake()
        {
            projectiles = FindObjectOfType<BulletController>().bulletPlayer;
        }

        private void LateUpdate()
        {
            if (photonView.IsMine)
            {
                HuyManager.Instance.SetUpTime(ref timeAttack);
                if (HuyManager.Instance.PlayerIsDeath() || HuyManager.Instance.GetPlayerIsHurt() ||
                    EventSystem.current.IsPointerOverGameObject())
                    return;
                if (timeAttack <= 0)
                {
                    if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.L))
                    {
                        photonView.RPC(nameof(BulletAttack), RpcTarget.All);
                        timeAttack = resetTimeAttack;
                    }
                }
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