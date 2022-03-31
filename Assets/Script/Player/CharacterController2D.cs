using UnityEngine;
using Game.Core;
using UnityEngine.SceneManagement;
using AnimationState = Game.Core.AnimationState;

namespace Game.Player
{
    public class CharacterController2D : BaseObject
    {
        [Header("Movement")] private const float MovementSmoothing = .05f;
        private Vector2 velocity = Vector2.zero;
        private float mHorizontal;
        [Space] [Header("Flip")] private bool mFacingRight = true;
        public float dashSpeed = 100f;
        private bool isDashing;
        private Transform groundCheck;
        private bool mGrounded;
        [Space] [SerializeField] public LayerMask whatIsGround;
        private const float GroundedRadius = .2f;
        public bool mDBJump;
        public float fallMultiplier = 5f;
        public float lowJumpMultiplier = 2f;
        private Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        private PlayerHealth playerHealth;
        private readonly AnimationState animationState = new AnimationState();

        public override void Awake()
        {
            base.Awake();
            playerHealth = GetComponent<PlayerHealth>();
            groundCheck = GameObject.Find("ground_check").transform;
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (playerHealth.PlayerIsDeath()) return;
            mHorizontal = Input.GetAxisRaw("Horizontal");
            if (!mGrounded || mDBJump == false)
            {
                if (Input.GetMouseButtonDown(1) && isDashing)
                {
                    Dash(mHorizontal);
                }
            }

            OnCollision();
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jumps();
            }
        }

        private void FixedUpdate()
        {
            if (!playerHealth.PlayerIsDeath())
            {
                Move(mHorizontal * playerHealth.playerData.movingSpeed * Time.fixedDeltaTime);
                var position = transform.position;
                position = new Vector3(Mathf.Clamp(position.x, clampMinX, clampMaxX), position.y, position.z);
                transform.position = position;
                if (Input.GetKey(KeyCode.G))
                {
                    SceneManager.LoadScene(2);
                }

                if (Input.GetKey(KeyCode.C))
                {
                    SceneManager.LoadScene(3);
                }
            }
            else
            {
                body.velocity = new Vector2(0, body.velocity.y);
                PlayerRun(0f);
            }
        }

        private void OnCollision()
        {
            mGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundedRadius, whatIsGround);
        }

        private void Move(float move)
        {
            var velocity1 = body.velocity;
            body.velocity = Vector2.SmoothDamp(velocity1, new Vector2(move * 10f, velocity1.y), ref velocity,
                MovementSmoothing);
            PlayerRun(Mathf.Abs(move));
            if (move > 0 && !mFacingRight)
            {
                Flip();
            }
            else if (move < 0 && mFacingRight)
            {
                Flip();
            }
        }

        private void Jumps()
        {
            if (mGrounded)
            {
                Jump();
                mGrounded = false;
                mDBJump = true;
                PlayerJump();
                isDashing = true;
            }
            else if (mDBJump)
            {
                Jump();
                mDBJump = false;
                PlayerJump();
                isDashing = true;
            }
        }

        private void Jump()
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(Vector2.up * playerHealth.playerData.jumpForce, ForceMode2D.Impulse);
            if (body.velocity.y < 0f)
            {
                body.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1f) * Time.deltaTime);
            }
            else if (body.velocity.y > 0f && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                body.velocity += Vector2.up * (Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.deltaTime);
            }

            //m_Rigidbody2D.velocity = Vector3.up * m_JumpForce;
        }

        private void Dash(float horizontal)
        {
            body.AddForce(Vector2.right * (horizontal * dashSpeed), ForceMode2D.Impulse);
            isDashing = false;
        }

        private void Flip()
        {
            mFacingRight = !mFacingRight;
            transform.Rotate(0f, 180f, 0f);
            // Vector3 theScale = transform.localScale;
            // theScale.x *= -1;
            // transform.localScale = theScale;
        }

        //Animator
        private void PlayerRun(float speed)
        {
            animator.SetFloat(animationState.playerRun, speed);
        }

        private void PlayerJump()
        {
            animator.SetTrigger(animationState.playerIsJump);
        }
    }
}