using Game.GamePlay;
using UnityEngine;

namespace Game.Player
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
        [SerializeField] private bool mGrounded;
        [SerializeField] private bool isJump;
        private bool mDBJump;
        public Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        [HideInInspector] public PlayerHealth playerHealth;
        private bool isOnCar;
        public bool isHurt;
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
            if (!HuyManager.PlayerIsDeath())
            {
                if (!isHurt)
                {
                    GetInput();
                    if (!mGrounded || mDBJump == false)
                    {
                        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1) && isDashing)
                        {
                            Dash(playerInput);
                        }
                    }
                }
            }
        }

        void GetInput()
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
                if (!isHurt)
                {
                    Move(playerInput * playerData.movingSpeed * Time.fixedDeltaTime);

                    if (isJump)
                    {
                        isJump = false;
                        Jumps();
                    }

                    animator.SetFloat("y_velocity", body.velocity.y);
                    if (Mathf.Abs(body.velocity.y) < 0.6f && mGrounded)
                    {
                        jumpCount = 0;
                        AnimPlayerJump();
                    }

                    Vector3 position = transform.position;
                    position = new Vector3(Mathf.Clamp(position.x, clampMinX, clampMaxX), position.y, position.z);
                    body.transform.position = position;
                }
            }
        }

        private void Move(float move)
        {
            Vector3 position = body.velocity;
            body.velocity = Vector2.SmoothDamp(position, new Vector2(move * 10f, position.y), ref velocity, MovementSmoothing);

            if (isOnCar || onWall)
            {
                PlayerRun(0f);
            }
            else
            {
                PlayerRun(Mathf.Abs(move));
            }

            if (move > 0f && !mFacingRight)
            {
                Flip();
            }
            else if (move < 0f && mFacingRight)
            {
                Flip();
            }
        }

        private void MobileMove(float move)
        {
            playerInput = move;
        }

        private void Jumps()
        {
            if (mGrounded)
            {
                Jump();
                mDBJump = true;
                isDashing = true;
            }
            else if (mDBJump)
            {
                Jump();
                mDBJump = false;
                isDashing = true;
            }

            AnimPlayerJump();
        }


        private void Jump()
        {
            if (mGrounded || jumpCount < 2f)
            {
                body.velocity = new Vector2(body.velocity.x, 0f);
                body.AddForce(Vector2.up * playerData.jumpForce, ForceMode2D.Impulse);
                AudioManager.instance.Play("Player_Jump");
                jumpCount++;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            EvaluateCollision(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            EvaluateCollision(other);
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            mGrounded = false;
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

        private void Dash(float horizontal)
        {
            body.AddForce(Vector2.right * (horizontal * playerData.dashSpeed), ForceMode2D.Impulse);
            isDashing = false;
        }

        private void Flip()
        {
            mFacingRight = !mFacingRight;
            // var position = transform;
            // var theScale = position.localScale;
            // theScale.x *= -1;
            // position.localScale = theScale;
            transform.Rotate(0f, 180f, 0f);
        }

        private void PlayerRun(float speed)
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
                playerData.movingSpeed -= 10;
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
                playerData.movingSpeed = startSpeed;
            }
        }

        // public IEnumerator KnockBack(float knockDuration, float knockPower, Vector3 knockDir)
        // {
        //     var timer = 0f;
        //     while (knockDuration > timer)
        //     {
        //         timer *= Time.deltaTime;
        //         var position = transform.position;
        //         var t = (position - knockDir).normalized;
        //         body.AddForce(new Vector3(t.x * knockPower, position.y, position.z));
        //     }
        //
        //     yield return null;
        // }

        // private void OnDrawGizmos()
        // {
        //     Gizmos.DrawSphere(groundCheck.position, GroundedRadius);
        // }

    }
}