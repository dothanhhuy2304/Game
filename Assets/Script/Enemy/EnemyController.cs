using System;
using System.Collections.Generic;
using Photon.Pun;
using Script.Core;
using Script.GamePlay;
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

    public abstract class EnemyController : MonoBehaviourPunCallbacks, IPunObservable
    {
        public EnemySetting enemySetting;
        [Header("Types")] [SerializeField] protected Rigidbody2D body;
        [SerializeField] protected Animator animator;
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] protected float movingSpeed;
        protected CharacterController2D playerCharacter;
        [SerializeField] private float offsetFlip;
        [Space] [Header("Time")] protected float currentTime;
        [SerializeField] protected float maxTimeAttack;
        [SerializeField] protected Transform offsetAttack;
        [SerializeField] protected Vector2 positionAttack;
        [HideInInspector] [SerializeField] protected Transform currentCharacterPos;
        protected bool isHitGrounds;


        protected virtual void Start()
        {
            playerCharacter = CharacterController2D.IsLocalPlayer;
        }

        protected void Flip()
        {
            Vector2 target = (currentCharacterPos.position - transform.position).normalized;
            float angle = Mathf.Atan2(target.x, target.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, angle + offsetFlip, 0));
        }

        protected void AttackBulletDirection()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(positionAttack);
            Vector2 direction = (currentCharacterPos.position - transform.position).normalized;
            projectiles[index].transform.rotation =
                Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            projectiles[index].Shoot(transform);
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected void AttackBullet()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.TransformPoint(positionAttack);
            projectiles[index].transform.rotation = Quaternion.identity;
            projectiles[index].Shoot(transform);
            AudioManager.instance.Play("Enemy_Attack_Shoot");
        }

        protected Transform FindClosestPlayer()
        {
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in HuyManager.Instance.listPlayerInGame)
            {
                if (!go) continue;
                var position = transform.position;
                var gos = go.transform.position;
                float currentDistance = (position - gos).magnitude;
                RaycastHit2D hit = Physics2D.Linecast(position, gos, GameManager.instance.playerMask);
                if (currentDistance < closestDistance)
                {
                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                    {
                        closestDistance = currentDistance;
                        //trans = go.transform;
                        trans = hit.collider.gameObject.transform;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return trans;
        }

        protected Transform FindClosetPlayerWithoutPhysic()
        {
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
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in HuyManager.Instance.listPlayerInGame)
            {
                if (!go) continue;
                var position = transform.position;
                var gos = go.transform.position;
                float currentDistance = (position - gos).magnitude;
                RaycastHit2D hit = Physics2D.Linecast(new Vector3(position.x, position.y + 0.3f, 0f), 
                        new Vector3(1 * 100f, position.y + 0.3f, 0f),
                    GameManager.instance.playerMask);
                RaycastHit2D hit2 = Physics2D.Linecast(new Vector3(position.x, position.y + 0.3f, 0f),
                    new Vector3(-1 * 100f, position.y + 0.3f, 0f),
                    GameManager.instance.playerMask);
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        //trans = go.transform;
                        trans = hit.collider.gameObject.transform;
                    }
                }
                else if (hit2.collider != null && hit2.collider.gameObject.CompareTag("Player"))
                {
                    if (currentDistance < closestDistance)
                    {
                        closestDistance = currentDistance;
                        //trans = go.transform;
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

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((Vector3) body.velocity);
                stream.SendNext((float) body.rotation);
                stream.SendNext((Vector3) transform.position);
                stream.SendNext((Quaternion) transform.rotation);
            }
            else
            {
                body.velocity = (Vector3) stream.ReceiveNext();
                body.rotation = (float) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.DrawLine(new Vector3(position.x, position.y + 0.3f, 0f), 
                new Vector3(1 * 100f, position.y + 0.3f, 0f));
        }
    }
}