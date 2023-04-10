using UnityEngine;
using Script.Core;
using Script.ScriptTable;

namespace Script.Player
{
    public class CharacterController2D : FastSingleton<CharacterController2D>
    {
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

        private void Start()
        {
            playerHealth = PlayerHealth.instance;
            startSpeed = playerData.movingSpeed;
        }

        private void Update()
        {
            if (!HuyManager.PlayerIsDeath() && !HuyManager.GetPlayerIsHurt())
            {
                PlayerInput();
                HuyManager.SetUpTime(ref timeNextDash);
                if (timeNextDash <= 0)
                {
                    if ((Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1)) && isDashing)
                    {
                        Dash(playerInput);
                        timeNextDash = 1.5f;
                    }
                }
            }
        }

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
            if (!HuyManager.PlayerIsDeath())
            {
                if (!HuyManager.GetPlayerIsHurt())
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

                    Move(playerInput * (startSpeed * Time.fixedDeltaTime));

                    if (isJump)
                    {
                        isJump = false;
                        Jump();
                    }

                    if (Mathf.Abs(body.velocity.y) < 0.6f && mGrounded)
                    {
                        jumpCount = 0;
                        AnimPlayerJump();
                    }

                    animator.SetFloat("y_velocity", body.velocity.y);
                }
            }
        }

        private void Move(float move)
        {
            Vector3 position = body.velocity;
            body.velocity = Vector2.SmoothDamp(position, new Vector2(move * 10f, position.y), ref velocity,
                MovementSmoothing);

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

        private void OnCollisionEnter2D(Collision2D other)
        {
            EvaluateCollision(other);
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

        private void Dash(float horizontal)
        {
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.right * (horizontal * playerData.dashSpeed), ForceMode2D.Impulse);
            isDashing = false;
        }

        private void Flip()
        {
            mFacingRight = !mFacingRight;
            transform.Rotate(0f, 180f, 0f);
            // var position = transform;
            // var theScale = position.localScale;
            // theScale.x *= -1;
            // position.localScale = theScale;
        }

        private void AnimPlayerRun(float speed)
        {
            animator.SetFloat("m_Run", speed);
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
            if (other.CompareTag("Grass"))
            {
                startSpeed -= 10f;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Car"))
            {
                isOnCar = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
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
}