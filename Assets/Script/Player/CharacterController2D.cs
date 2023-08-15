using System;
using Photon.Pun;
using UnityEngine;
using Script.Core;
using Script.ScriptTable;
using Unity.Mathematics;

namespace Script.Player
{
    public class CharacterController2D : MonoBehaviourPunCallbacks, IPunObservable
    {
        public static CharacterController2D IsLocalPlayer;
        public Rigidbody2D body;
        public Collider2D col;
        public Data playerData;
        [Header("Movement")] private const float MovementSmoothing = .05f;
        private Vector2 velocity = Vector2.zero;
        private float playerInput;

        [Space] [Header("Flip")] private bool mFacingRight = true;
        private bool isDashing;
        private float timeNextDash = 1.5f;
        [SerializeField] private bool mGrounded;
        [SerializeField] private bool isJump;
        private bool mDbJump;
        public Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        [HideInInspector] public PlayerHealth playerHealth;
        private bool isOnCar;
        private float startSpeed;
        private int jumpCount;
        private bool onWall;
        private static readonly int Velocity = Animator.StringToHash("y_velocity");
        private static readonly int MRun = Animator.StringToHash("m_Run");
        private static readonly int IsJump = Animator.StringToHash("is_Jump");
        private static readonly int IsDbJump = Animator.StringToHash("is_DBJump");

        private void Awake()
        {
            if (photonView.IsMine)
            {
                IsLocalPlayer = GetComponent<CharacterController2D>();
            }

            HuyManager.Instance.listPlayerInGame = FindObjectsOfType<CharacterController2D>();
        }
        
        private void Start()
        {
            if (photonView.IsMine)
            {
                playerHealth = FindObjectOfType<PlayerHealth>();
                startSpeed = playerData.movingSpeed;
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (!HuyManager.Instance.PlayerIsDeath() && !HuyManager.Instance.GetPlayerIsHurt())
                {
                    photonView.RPC(nameof(PlayerInput), RpcTarget.All);
                    HuyManager.Instance.SetUpTime(ref timeNextDash);
                    if (timeNextDash <= 0)
                    {
                        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)) && isDashing)
                        {
                            photonView.RPC(nameof(Dash), RpcTarget.All, playerInput);
                        }
                    }
                }
            }
        }

        [PunRPC]
        private void PlayerInput()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            playerInput = Input.GetAxisRaw("Horizontal");
            isJump |= Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
#elif !UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
             playerInput = Input.GetAxisRaw("Vertical");
             isJump |= Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow);
#endif
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (!HuyManager.Instance.PlayerIsDeath() && !HuyManager.Instance.GetPlayerIsHurt())
                {
                    photonView.RPC(nameof(RpcCheckGround), RpcTarget.All);
                    ControlPc(playerInput * (startSpeed * Time.fixedDeltaTime));

                    if (isJump)
                    {
                        Jump();
                    }

                    if (Mathf.Abs(body.velocity.y) < 0.6f && mGrounded)
                    {
                        photonView.RPC(nameof(RpcResetAnimJump), RpcTarget.All);
                    }

                    photonView.RPC(nameof(YVelocity), RpcTarget.All);
                }
            }
        }

        [PunRPC]
        private void RpcCheckGround()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, 1 << LayerMask.NameToLayer("ground"));
            if (hit)
            {
                if (!hit.collider.CompareTag("ground"))
                {
                    mGrounded = false;
                }
                else
                {
                    mGrounded = true;
                }
            }
        }

        [PunRPC]
        private void YVelocity()
        {
            animator.SetFloat(Velocity, body.velocity.y);
        }

        private void ControlPc(float input)
        {
            photonView.RPC(nameof(Moving), RpcTarget.All, input);

            if (input > 0f && !mFacingRight)
            {
                Flip();
            }
            else if (input < 0f && mFacingRight)
            {
                Flip();
            }

            MovementLimit();
        }

        [PunRPC]
        private void Moving(float @fixed)
        {
            Vector3 position = body.velocity;
            body.velocity = Vector2.SmoothDamp(position, new Vector2(@fixed * 10f, position.y), ref velocity, MovementSmoothing);

            if (isOnCar || onWall)
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
            Vector3 pos = transform.position;
            pos = new Vector3(Mathf.Clamp(pos.x, clampMinX, clampMaxX), pos.y, pos.z);
            body.transform.position = pos;
        }

        private void MobileMove(float move)
        {
            playerInput = move;
        }

        private void Jump()
        {
            isJump = false;
            if (mGrounded)
            {
                photonView.RPC(nameof(JumpForce), RpcTarget.All);
                mDbJump = true;
            }
            else if (mDbJump)
            {
                photonView.RPC(nameof(JumpForce), RpcTarget.All);
                mDbJump = false;
            }

            photonView.RPC(nameof(JumpAnimation), RpcTarget.All);
            mGrounded = false;
            isDashing = true;
        }

        [PunRPC]
        private void JumpForce()
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
            AudioManager.instance.Play("Player_Jump");
            jumpCount++;
        }

        [PunRPC]
        private void JumpAnimation()
        {
            switch (jumpCount)
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

            onWall = isWall;
        }

        [PunRPC]
        private void Dash(float horizontal)
        {
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.right * (horizontal * playerData.dashSpeed), ForceMode2D.Impulse);
            isDashing = false;
            timeNextDash = 1.5f;
        }

        private void Flip()
        {
            mFacingRight = !mFacingRight;
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
            jumpCount = 0;
            animator.SetBool(IsJump, false);
            animator.SetBool(IsDbJump, false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (photonView.IsMine)
            {
                if (other.CompareTag("Grass"))
                {
                    startSpeed -= 10f;
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (photonView.IsMine)
            {
                if (other.CompareTag("Car"))
                {
                    isOnCar = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (photonView.IsMine)
            {
                if (other.CompareTag("Car"))
                {
                    isOnCar = false;
                }

                if (other.CompareTag("Grass"))
                {
                    startSpeed = playerData.movingSpeed;
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((float) body.rotation);
                stream.SendNext((Vector3) body.velocity);
                stream.SendNext((Vector3) transform.position);
                stream.SendNext((Quaternion) transform.rotation);
                stream.SendNext((float) playerData.movingSpeed);
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