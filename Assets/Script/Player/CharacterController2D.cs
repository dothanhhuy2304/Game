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
        public static CharacterController2D instance;
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

        private void Awake()
        {
            if (photonView.IsMine)
            {
                if (instance == null)
                {
                    instance = this;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        
        private void Start()
        {
            if (photonView.IsMine)
            {
                playerHealth = PlayerHealth.instance;
                startSpeed = playerData.movingSpeed;
            }
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                //if (!HuyManager.Instance.PlayerIsDeath() && !HuyManager.Instance.GetPlayerIsHurt())
                {
                    photonView.RPC(nameof(PlayerInput), RpcTarget.All);
                    HuyManager.Instance.SetUpTime(ref timeNextDash);
                    if (timeNextDash <= 0)
                    {
                        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)) && isDashing)
                        {
                            photonView.RPC(nameof(Dash), RpcTarget.All, playerInput);
                            timeNextDash = 1.5f;
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
                //if (!HuyManager.Instance.PlayerIsDeath())
                {
                    //if (!HuyManager.Instance.GetPlayerIsHurt())
                    {
                        photonView.RPC(nameof(RpcCheckGround), RpcTarget.All);

                        photonView.RPC(nameof(Move), RpcTarget.All, playerInput * (startSpeed * Time.fixedDeltaTime));
                        
                        if (isJump)
                        {
                            isJump = false;
                            photonView.RPC(nameof(Jump), RpcTarget.All);
                        }

                        photonView.RPC(nameof(RpcJumpAnim), RpcTarget.All);

                        photonView.RPC(nameof(YVelocity), RpcTarget.All);
                    }
                }
            }
        }

        [PunRPC]
        private void RpcJumpAnim()
        {
            if (Mathf.Abs(body.velocity.y) < 0.6f && mGrounded)
            {
                jumpCount = 0;
                RpcResetAnimJump();
            }
        }

        [PunRPC]
        private void RpcCheckGround()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f,
                1 << LayerMask.NameToLayer("ground"));
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
            animator.SetFloat("y_velocity", body.velocity.y);
        }

        [PunRPC]
        private void Move(float move)
        {
            Vector3 position = body.velocity;
            body.velocity = Vector2.SmoothDamp(position, new Vector2(move * 10f, position.y) , ref velocity, MovementSmoothing);

            if (isOnCar || onWall)
            {
                AnimPlayerRun(0f);
            }
            else
            {
                AnimPlayerRun(Mathf.Abs(move));
            }

            if (move > 0f && !mFacingRight)
            {
                Flip();
            }
            else if (move < 0f && mFacingRight)
            {
                Flip();
            }

            LockPlayerPositionOnMap();
        }

        private void LockPlayerPositionOnMap()
        {
            Vector3 pos = transform.position;
            pos = new Vector3(Mathf.Clamp(pos.x, clampMinX, clampMaxX), pos.y, pos.z);
            body.transform.position = pos;
        }

        private void MobileMove(float move)
        {
            playerInput = move;
        }

        [PunRPC]
        private void Jump()
        {
            if (mGrounded)
            {
                SetUpJump();
                mDbJump = true;
            }
            else if (mDbJump)
            {
                SetUpJump();
                mDbJump = false;
            }

            mGrounded = false;
            isDashing = true;
            AnimPlayerJump();
        }


        private void SetUpJump()
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
            AudioManager.instance.Play("Player_Jump");
            jumpCount++;
        }

        private void ModifyPhysics()
        {
            
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (photonView.IsMine)
            {
                EvaluateCollision(other);
            }
        }

        #region If the player stands on the ground and no raycast, We can use this function

        //private void OnCollisionStay2D(Collision2D other)
        //{
        //EvaluateCollision(other);
        //}

        //private void OnCollisionExit2D(Collision2D other)
        //{
        //mGrounded = false;
        //}

        #endregion

        [PunRPC]
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
        private void AnimPlayerRun(float speed)
        {
            animator.SetFloat("m_Run", speed);
        }

        [PunRPC]
        private void RpcResetAnimJump()
        {
            animator.SetBool("is_Jump", false);
            animator.SetBool("is_DBJump", false);
        }


        private void AnimPlayerJump()
        {
            switch (jumpCount)
            {
                case 0:
                    animator.SetBool("is_Jump", false);
                    animator.SetBool("is_DBJump", false);
                    break;
                case 1:
                    animator.SetBool("is_Jump", true);
                    break;
                case 2:
                    animator.SetBool("is_Jump", false);
                    animator.SetBool("is_DBJump", true);
                    break;
            }
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
            }
            else
            {
                body.rotation = (float) stream.ReceiveNext();
                body.velocity = (Vector3) stream.ReceiveNext();
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = (Quaternion) stream.ReceiveNext();
                //float lag = Mathf.Abs((float) (PhotonNetwork.Time - info.SentServerTime));
                //networkPosition += (body.velocity * lag);
            }
        }
    }
}