using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;
using UnityEngine.Serialization;

namespace Script.Player
{
    public class PetAI : MonoBehaviourPun, IPunObservable
    {
        public static PetAI IsLocalPet;
        [SerializeField] private PhotonView pv;
        public Data petData;
        [SerializeField] private SpriteRenderer petRenderer;
        [SerializeField] private Rigidbody2D body;
        [SerializeField] private List<FireProjectile> projectiles;
        [SerializeField] [HideInInspector] private List<GameObject> listEnemyInMap;
        [HideInInspector] public Transform closestEnemy;
        private bool _enemyContact;
        [SerializeField] private float distancePlayer;
        [SerializeField] private float rangeAttack;
        private float _timeAttack = 3f;
        private const float MaxTimeAttack = 3f;
        [SerializeField] private Animator animator;
        private readonly int _isRun = Animator.StringToHash("isRun");
        private readonly int _isAttack = Animator.StringToHash("isAttack");
        private bool _checkHitGround;
        private int _tempIndex;
        private bool _canAttack;
        private CharacterController2D _character;

        private void Awake()
        {
            if (pv == null)
            {
                pv = GetComponent<PhotonView>();
            }

            if (pv.IsMine)
            {
                IsLocalPet = GetComponent<PetAI>();
                petRenderer.sortingOrder += pv.Owner.ActorNumber;
            }

            _character = CharacterController2D.IsLocalPlayer;
            projectiles = FindObjectOfType<BulletController>().petAi;
            listEnemyInMap = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        }

        private void Update()
        {
            HuyManager.Instance.SetUpTime(ref _timeAttack);
            if (!_character.playerHealth.isDeath)
            {
                if ((_character.transform.position - transform.position).magnitude > distancePlayer)
                {
                    if (pv.IsMine)
                    {
                        animator.SetBool(_isRun, true);
                        Vector2 playerPos = _character.transform.position;
                        transform.DOMove(new Vector2(playerPos.x + 2f, playerPos.y + 1f), 0.5f)
                            .SetEase(Ease.Linear);
                    }
                }

                CheckAttack();
            }
        }

        private void CheckAttack()
        {
            FindEnemyCloset();
            if (_canAttack)
            {
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
                    if ((closestEnemy.transform.position - transform.position).magnitude < rangeAttack)
                    {
                        pv.RPC(nameof(RpcShot), RpcTarget.AllBuffered);
                    }
                }
            }
        }

        private void FindEnemyCloset()
        {
            closestEnemy = FindClosestEnemy();
            _canAttack = closestEnemy;
        }

        [PunRPC]
        private void RpcShot()
        {
            if (_timeAttack <= 0f)
            {
                animator.SetTrigger(_isAttack);
                DurationAttack();
                _timeAttack = MaxTimeAttack;
            }
        }

        private void DurationAttack()
        {
            DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(BulletAttack);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemyContact = !_checkHitGround;
            }
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                _enemyContact = false;
            }
        }

        private void BulletAttack()
        {
            int index = FindBullet();
            projectiles[index].transform.position = transform.position;
            projectiles[index].transform.rotation = transform.rotation;
            projectiles[index].Shoot(transform, closestEnemy);
        }

        private Transform FindClosestEnemy()
        {
            if (listEnemyInMap.Count <= 0)
            {
                return null;
            }
            
            float closestDistance = Mathf.Infinity;
            Transform trans = null;
            foreach (var go in listEnemyInMap)
            {
                if (!go) continue;
                Vector3 location = transform.position;
                Vector3 gos = go.transform.position;
                float currentDistance = (location - gos).magnitude;
                RaycastHit2D hit = Physics2D.Linecast(location, gos, LayerMaskManager.instance.enemyMask);
                if (currentDistance < closestDistance)
                {
                    if (hit.collider && hit.collider.gameObject.CompareTag("Enemy"))
                    {
                        closestDistance = currentDistance;
                        trans = hit.collider.gameObject.transform;
                    }
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
                stream.SendNext(body.rotation);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
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