using System;
using Photon.Pun;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;
using UnityEngine.Serialization;

namespace Script.Player
{
    public class CharacterController2D : MonoBehaviourPun, IPunObservable
    {
        public static CharacterController2D IsLocalPlayer;
        public Rigidbody2D body;
        public Collider2D col;
        [SerializeField] private SpriteRenderer[] playerRenderer;
        [SerializeField] private TMPro.TMP_Text playerName;
        public Data playerData;
        [Header("Movement")] private const float MovementSmoothing = .05f;
        private Vector2 _velocity = Vector2.zero;
        private float _playerInput;

        [Space] [Header("Flip")] private bool _mFacingRight = true;
        private bool _isDashing;
        [SerializeField] private bool mGrounded;
        [SerializeField] private bool isJump;
        private bool _mDbJump;
        public Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        [SerializeField] public PlayerHealth playerHealth;
        private bool _isOnCar;
        private float _startSpeed;
        private int _jumpCount;
        private bool _onWall;
        private static readonly int Velocity = Animator.StringToHash("y_velocity");
        private static readonly int MRun = Animator.StringToHash("m_Run");
        private static readonly int IsJump = Animator.StringToHash("is_Jump");
        private static readonly int IsDbJump = Animator.StringToHash("is_DBJump");

        [HideInInspector] public MobileInputManager mobileInput;

        private void Awake()
        {
            mobileInput = FindObjectOfType<MobileInputManager>();
            mobileInput.btnDash.onClick.AddListener(MobileDash);
            mobileInput.btnJump.onClick.AddListener(MobileJump);
            if (photonView.IsMine)
            {
                IsLocalPlayer = GetComponent<CharacterController2D>();
                _startSpeed = playerData.movingSpeed;
            }

            playerRenderer[0].sortingOrder = photonView.Owner.ActorNumber;
            playerRenderer[1].sortingOrder = photonView.Owner.ActorNumber;
            playerName.text = HuyManager.GetCurrentPlayerProfile().UserName;
            playerData.startGravity = body.gravityScale;

            //HuyManager.Instance.listPlayerInGame = FindObjectsOfType<CharacterController2D>();
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (!playerHealth.isHurt && !playerHealth.isDeath)
                {
                    photonView.RPC(nameof(PlayerInput), RpcTarget.AllBuffered);
                    HuyManager.Instance.SetUpTime(ref playerData.timeToDash);
#if UNITY_STANDALONE
                    if (playerData.timeToDash <= 0)
                    {
                        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)) && _isDashing && !mGrounded)
                        {
                            photonView.RPC(nameof(Dash), RpcTarget.AllBuffered, _playerInput);
                        }
                    }
#endif
                }
            }
        }

        [PunRPC]
        private void PlayerInput()
        {
#if UNITY_STANDALONE
            _playerInput = Input.GetAxisRaw("Horizontal");
            isJump |= Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
#elif UNITY_ANDROID || UNITY_IOS
            if (mobileInput.joystick.Horizontal < 0)
            {
                _playerInput = -1;
            }
            else if (mobileInput.joystick.Horizontal > 0)
            {
                _playerInput = 1;
            }
            else
            {
                _playerInput = 0;
            }
#endif
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (!playerHealth.isDeath && !playerHealth.isHurt)
                {
                    photonView.RPC(nameof(RpcCheckGround), RpcTarget.AllBuffered);
                    ControlPlayer(_playerInput * (_startSpeed * Time.fixedDeltaTime));

                    if (isJump)
                    {
                        photonView.RPC(nameof(Jump), RpcTarget.AllBuffered);
                    }

                    if (Mathf.Abs(body.velocity.y) < 0.6f && mGrounded)
                    {
                        photonView.RPC(nameof(RpcResetAnimJump), RpcTarget.AllBuffered);
                    }

                    photonView.RPC(nameof(YVelocity), RpcTarget.AllBuffered);
                }
            }
        }


        private void MobileDash()
        {
            if (playerData.timeToDash <= 0)
            {
                if (_isDashing && !mGrounded)
                {
                    photonView.RPC(nameof(Dash), RpcTarget.AllBuffered, _playerInput);
                }
            }
        }

        private void MobileJump()
        {
            isJump = true;
        }

        [PunRPC]
        private void RpcCheckGround()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f,
                1 << LayerMask.NameToLayer("ground"));
            if (hit)
            {
                mGrounded = hit.collider.CompareTag("ground");
            }
        }

        [PunRPC]
        private void YVelocity()
        {
            animator.SetFloat(Velocity, body.velocity.y);
        }

        private void ControlPlayer(float input)
        {
            photonView.RPC(nameof(Moving), RpcTarget.AllBuffered, input);

            if (input > 0f && !_mFacingRight)
            {
                Flip();
            }
            else if (input < 0f && _mFacingRight)
            {
                Flip();
            }

            MovementLimit();
        }

        [PunRPC]
        private void Moving(float @fixed)
        {
            Vector3 position = body.velocity;
            body.velocity = Vector2.SmoothDamp(position, new Vector2(@fixed * 10f, position.y), ref _velocity,
                MovementSmoothing);

            if (_isOnCar || _onWall)
            {
                RunAnimation(0f);
            }
            else
            {
                RunAnimation(Mathf.Abs(@fixed));
            }
        }

        private void RunAnimation(float value)
        {
            animator.SetFloat(MRun, value);
        }

        private void MovementLimit()
        {
            Vector3 playerPosition = transform.position;
            playerPosition = new Vector3(Mathf.Clamp(playerPosition.x, clampMinX, clampMaxX), playerPosition.y,
                playerPosition.z);
            body.transform.position = playerPosition;
        }

        [PunRPC]
        private void Jump()
        {
            isJump = false;
            if (mGrounded)
            {
                JumpForce();
                _mDbJump = true;
            }
            else if (_mDbJump && !mGrounded)
            {
                JumpForce();
                _mDbJump = false;
            }

            JumpAnimation();
            mGrounded = false;
            _isDashing = true;
        }

        private void JumpForce()
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
            AudioManager.instance.Play("Player_Jump");
            _jumpCount++;
        }
        
        private void JumpAnimation()
        {
            switch (_jumpCount)
            {
                case 0:
                    animator.SetBool(IsJump, false);
                    animator.SetBool(IsDbJump, false);
                    break;
                case 1:
                    animator.SetBool(IsJump, true);
                    break;
                case 2:
                    animator.SetBool(IsJump, false);
                    animator.SetBool(IsDbJump, true);
                    break;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (photonView.IsMine)
            {
                EvaluateCollision(other);
            }
        }
        
        private void EvaluateCollision(Collision2D collision2D)
        {
            bool isWall = false;
            for (int i = 0; i < collision2D.contactCount; i++)
            {
                Vector3 normal = collision2D.GetContact(i).normal;
                mGrounded |= normal.y > 0.6f;
                if (!collision2D.collider.CompareTag("ground"))
                {
                    isWall = true;
                }
            }

            _onWall = isWall;
        }


        [PunRPC]
        private void Dash(float horizontal)
        {
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.right * (horizontal * playerData.dashSpeed), ForceMode2D.Impulse);
            body.gravityScale = 0;
            _isDashing = false;
            playerData.timeToDash = playerData.timeEndDash;
            isJump = false;
            _mDbJump = false;
            Invoke(nameof(BodyType),0.2f);
        }
        
        private void BodyType()
        {
            body.gravityScale = playerData.startGravity;
        }

        private void Flip()
        {
            _mFacingRight = !_mFacingRight;
            Vector3 dir = new Vector3(0, 180f, 0);
            transform.Rotate(dir);
            // var position = transform;
            // var theScale = position.localScale;
            // theScale.x *= -1;
            // position.localScale = theScale;
        }

        [PunRPC]
        private void RpcResetAnimJump()
        {
            _jumpCount = 0;
            animator.SetBool(IsJump, false);
            animator.SetBool(IsDbJump, false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (photonView.IsMine)
            {
                if (other.CompareTag("Grass"))
                {
                    _startSpeed -= 10f;
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (photonView.IsMine)
            {
                if (other.CompareTag("Car"))
                {
                    _isOnCar = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (photonView.IsMine)
            {
                if (other.CompareTag("Car"))
                {
                    _isOnCar = false;
                }

                if (other.CompareTag("Grass"))
                {
                    _startSpeed = playerData.movingSpeed;
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(body.rotation);
                stream.SendNext((Vector3) body.velocity);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(playerData.movingSpeed);
            }
            else
            {
                body.rotation = (float) stream.ReceiveNext();
                body.velocity = (Vector3) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
                playerData.movingSpeed = (float) stream.ReceiveNext();
                //float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
                //networkPosition += (body.velocity * lag);
            }
        }
    }
}