using System;
using System.Collections.Generic;
using Photon.Pun;
using Script.Core;
using Script.Player;
using UnityEngine;

namespace Script.Enemy
{

    [Serializable]
    public class EnemySetting
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public bool isStun;
        public bool canAttack;
        public EnemyHealth enemyHeal;
        public float rangeAttack;
        public bool canMoving;
    }

    public abstract class EnemyController : MonoBehaviourPun
    {
        public EnemySetting enemySetting;
        [SerializeField] protected Rigidbody2D body;
        [SerializeField] protected Animator animator;
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] protected float movingSpeed;
        [SerializeField] private float offsetFlip;
        [Space] 
        protected float currentTime = 0;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] protected Transform offsetAttack;
        [SerializeField] protected Vector2 positionAttack;
        [HideInInspector] [SerializeField] protected Transform currentCharacterPos;

        protected void Flip()
        {
            Vector2 target = (currentCharacterPos.position - transform.position).normalized;
            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, angle + offsetFlip, 0));
        }

        protected void AttackBullet(bool useDirection = false)
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(positionAttack);
            if (!useDirection)
            {
                projectiles[index].transform.rotation = Quaternion.identity;
                projectiles[index].Shoot(transform);
            }
            else
            {
                Vector2 direction = currentCharacterPos.position - transform.position;
                projectiles[index].transform.rotation =
                    Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                projectiles[index].Shoot(transform, currentCharacterPos);
            }

            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBulletByPlayer(Transform trans)
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(positionAttack);
            projectiles[index].transform.rotation = transform.rotation;
            projectiles[index].Shoot(trans);
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected Transform FindClosestPlayer()
        {
            if (HuyManager.Instance.listPlayerInGame.Length <= 0)
            {
                return null;
            }
                
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in HuyManager.Instance.listPlayerInGame)
            {
                if (!go) continue;
                var position = transform.position;
                var gos = go.transform.position;
                float currentDistance = (position - gos).magnitude;
                RaycastHit2D hit = Physics2D.Linecast(position, gos, LayerMaskManager.instance.playerMask);
                if (currentDistance < closestDistance)
                {
                    if (hit.collider != null)
                    {
                        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                        if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.GetComponent<PlayerHealth>().isDeath)
                        {
                            closestDistance = currentDistance;
                            trans = hit.collider.gameObject.transform;
                        }
                    }
                }
            }

            return trans;
        }

        protected Transform FindPlayerClosetWithLocalPlayer()
        {
            Vector3 target = CharacterController2D.IsLocalPlayer.transform.position;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, target, LayerMaskManager.instance.playerMask);
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                return hit.collider.gameObject.transform;
            }

            return null;
        }

        protected Transform FindClosetPlayerWithoutPhysic()
        {
            if (HuyManager.Instance.listPlayerInGame.Length <= 0)
            {
                return null;
            }
            
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in HuyManager.Instance.listPlayerInGame)
            {
                if (!go) continue;
                var position = transform.position;
                var gos = go.transform.position;
                float currentDistance = (position - gos).magnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trans = go.transform;
                }
            }

            return trans;
        }

        protected Transform FindClosetPlayerWithForwardPhysic()
        {
            if (HuyManager.Instance.listPlayerInGame.Length <= 0)
            {
                return null;
            }
            
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in HuyManager.Instance.listPlayerInGame)
            {
                if (!go) continue;
                var position = transform.position;
                var gos = go.transform.position;
                float currentDistance = (position - gos).magnitude;
                RaycastHit2D hit = Physics2D.Linecast(new Vector3(position.x, position.y + 0.3f, 0f),
                    new Vector3(1 * 100f, position.y + 0.3f, 0f), LayerMaskManager.instance.playerMask);
                RaycastHit2D hit2 = Physics2D.Linecast(new Vector3(position.x, position.y + 0.3f, 0f),
                    new Vector3(-1 * 100f, position.y + 0.3f, 0f), LayerMaskManager.instance.playerMask);
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                if (hit.collider && hit.collider.gameObject.CompareTag("Player") && !hit.collider.GetComponent<PlayerHealth>().isDeath)
                {
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        trans = hit.collider.gameObject.transform;
                    }
                }
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                else if (hit2.collider && hit2.collider.gameObject.CompareTag("Player") && !hit2.collider.GetComponent<PlayerHealth>().isDeath)
                {
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        trans = hit2.collider.gameObject.transform;
                    }
                }
                else
                {
                    return null;
                }
            }

            return trans;
        }

        private int _tempIndex;

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

    }
}