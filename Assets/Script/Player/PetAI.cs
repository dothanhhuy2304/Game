using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;
using Random = UnityEngine.Random;

namespace Script.Player
{

    public class PetAI : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static PetAI IsLocalPet;
        public Data petData;
        [SerializeField] private Rigidbody2D body;
        //private Vector2 velocity = Vector2.zero;
        [SerializeField] private List<FireProjectile> projectiles;
        private List<GameObject> _listEnemyInMap;
        [HideInInspector] public Transform closestEnemy;
        private bool _enemyContact;
        [SerializeField] private float distancePlayer;
        [SerializeField] private float rangeAttack;
        private float currentTimeAttack = 3f;
        private const float TimeAttack = 3f;
        [SerializeField] private Animator animator;
        private static readonly int IsRun = Animator.StringToHash("isRun");
        private bool _checkHitGround;
        private int _tempIndex;
        private float distanceToPlay;

        private void Awake()
        {
            if (photonView.IsMine)
            {
                IsLocalPet = GetComponent<PetAI>();
            }

            projectiles = FindObjectOfType<BulletController>().petAi;
        }

        private void Start()
        {
            _listEnemyInMap = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        }
        
        private void Update()
        {
            HuyManager.Instance.SetUpTime(ref currentTimeAttack);
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (!HuyManager.Instance.PlayerIsDeath())
                {
                    if ((CharacterController2D.IsLocalPlayer.transform.position - transform.position).magnitude > distancePlayer)
                    {
                        photonView.RPC(nameof(MoveToPlayer), RpcTarget.All);
                        CheckAttack();
                    }
                    else
                    {
                        CheckAttack();
                        body.velocity = Vector2.zero;
                    }

                }
            }
        }

        private void CheckAttack()
        {
            closestEnemy = FindClosestEnemy();
            RaycastHit2D hit = Physics2D.Linecast(transform.position, closestEnemy.transform.position, 1 << LayerMask.NameToLayer("ground"));
            if (hit)
            {
                if (hit.collider.CompareTag("ground"))
                {
                    _checkHitGround = true;
                    return;
                }
            }

            _checkHitGround = false;
            if (_enemyContact)
            {
                if (Vector2.Distance(transform.position, closestEnemy.position) < rangeAttack)
                {
                    if (currentTimeAttack <= 0f)
                    {
                        photonView.RPC(nameof(BulletAttack), RpcTarget.All);
                        currentTimeAttack = TimeAttack;
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                if (!_checkHitGround)
                {
                    closestEnemy.GetComponentInChildren<SpriteRenderer>()?.DOColor(new Color(1f, 0.6f, 0.5f),0.3f);
                    _enemyContact = true;
                }
                else
                {
                    closestEnemy.GetComponentInChildren<SpriteRenderer>()?.DOColor(Color.white, 0.3f);
                    _enemyContact = false;
                }
            }
        }
        

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                other.GetComponentInChildren<SpriteRenderer>()?.DOColor(Color.white, 0.3f);
                _enemyContact = false;
            }
        }
        
        [PunRPC]
        private void MoveToPlayer()
        {
            //Vector2 angle = (_player.transform.position - transform.position).normalized;
            //body.velocity = Vector2.SmoothDamp(body.velocity, angle * petData.movingSpeed, ref _velocity, .05f);
            Vector2 playerPos = CharacterController2D.IsLocalPlayer.transform.position;
            //transform.DOMove(new Vector3(playerPos.x + Random.Range(- 2f , 2f), playerPos.y + 1), 0.5f).SetEase(Ease.Linear);
            //Vector2 target = (_player.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(playerPos.x + 1, playerPos.y + 1), 10 * Time.deltaTime);
            animator.SetBool(IsRun, true);

        }

        [PunRPC]
        private void BulletAttack()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.position;
            projectiles[index].transform.rotation = transform.rotation;
            projectiles[index].Shoot(transform);
            animator.SetBool(IsRun, false);
        }

        private Transform FindClosestEnemy()
        {
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in _listEnemyInMap)
            {
                if (!go) continue;
                float currentDistance = (transform.position - go.transform.position).magnitude;
                if (currentDistance < closestDistance)
                {
                    closestDistance = currentDistance;
                    trans = go.transform;
                }
            }

            return trans;
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
    }
}