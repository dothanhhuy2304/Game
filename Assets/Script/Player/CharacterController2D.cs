using UnityEngine;
using Game.Core;
using UnityEngine.SceneManagement;

namespace Game.Player
{
    public class CharacterController2D : BaseObject
    {
        [Header("Movement")] private const float MovementSmoothing = .05f;
        private Vector2 velocity = Vector2.zero;
        private float mHorizontal;
        [Space] [Header("Flip")] private bool mFacingRight = true;
        //public float dashSpeed = 100f;
        private bool isDashing;
        private bool mGrounded;
        [Space] [SerializeField] public LayerMask whatIsGround;
        private bool mDBJump;
        private Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        private PlayerHealth playerHealth;
        private readonly AnimationStates animationState = new AnimationStates();
        private Collider2D controllerCollider;
        private PlayerAudio playerAudio;

        public override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            controllerCollider = GetComponent<Collider2D>();
            playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
            playerHealth = GetComponent<PlayerHealth>();
        }

        private void Update()
        {
            if (playerHealth.PlayerIsDeath()) return;
            mHorizontal = Input.GetAxisRaw("Horizontal");
            if (!mGrounded || mDBJump == false)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1) && isDashing)
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
                body.transform.position = position;
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
                body.velocity = new Vector2(0f, body.velocity.y);
                PlayerRun(0f);
            }
        }

        private void OnCollision()
        {
            mGrounded = controllerCollider.IsTouchingLayers(whatIsGround);
        }

        private void Move(float move)
        {
            var velocity1 = body.velocity;
            body.velocity = Vector2.SmoothDamp(velocity1, new Vector2(move * 10f, velocity1.y), ref velocity,
                MovementSmoothing);
            PlayerRun(Mathf.Abs(move));
            if (move > 0f && !mFacingRight)
            {
                Flip();
            }
            else if (move < 0f && mFacingRight)
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
                playerAudio.PlayerJump();
            }
            else if (mDBJump)
            {
                Jump();
                mDBJump = false;
                PlayerJump();
                isDashing = true;
                playerAudio.PlayerJump();
            }
        }

        private void Jump()
        {
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(Vector2.up * playerHealth.playerData.jumpForce, ForceMode2D.Impulse);
        }

        private void Dash(float horizontal)
        {
            body.AddForce(Vector2.right * (horizontal * playerHealth.playerData.dashSpeed), ForceMode2D.Impulse);
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

    public enum JumpState
    {
        Normal,
        DoubleJump,
        None
    }
}