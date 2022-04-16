using System.Collections;
using UnityEngine;
using Game.Core;

namespace Game.Player
{
    public class CharacterController2D : BaseObject
    {
        [Header("Movement")] private const float MovementSmoothing = .05f;
        private Vector2 velocity = Vector2.zero;
        private float mHorizontal;

        [Space] [Header("Flip")] private bool mFacingRight = true;
        private bool isDashing;
        private bool mGrounded;
        private const float GroundedRadius = .3f;
        [SerializeField] private Transform groundCheck;
        [Space] [SerializeField] private LayerMask whatIsGround;
        private bool mDBJump;
        [SerializeField] private Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        [SerializeField] private PlayerHealth playerHealth;
        private readonly AnimationStates animationState = new AnimationStates();
        private PlayerAudio playerAudio;
        private bool isOnCar;
        public bool isHurt;
        private float startSpeed;

        protected override void Start()
        {
            base.Start();
            if (!groundCheck)
            {
                groundCheck = GameObject.Find("ground_check").transform;
            }

            playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
            if (!playerHealth)
            {
                playerHealth = GetComponent<PlayerHealth>();
            }

            startSpeed = playerHealth.playerData.movingSpeed;
        }

        protected override void Update()
        {
            if (playerHealth.PlayerIsDeath()) return;
            if (CheckGrass())
            {
                playerHealth.playerData.movingSpeed /= 2;
            }
            else
            {
                playerHealth.playerData.movingSpeed = startSpeed;
            }

            if (isHurt) return;
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

        protected override void FixedUpdate()
        {
            if (!playerHealth.PlayerIsDeath())
            {
                if (isHurt) return;
                Move(mHorizontal * playerHealth.playerData.movingSpeed * Time.fixedDeltaTime);
                var position = transform.position;
                position = new Vector3(Mathf.Clamp(position.x, clampMinX, clampMaxX), position.y, position.z);
                body.transform.position = position;
            }
            else
            {
                if (isHurt) return;
                body.velocity = new Vector2(0f, body.velocity.y);
                PlayerRun(0f);
            }
        }

        private void OnCollision()
        {
            mGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundedRadius, whatIsGround);
        }

        private bool CheckHitWall()
        {
            var transform1 = transform;
            return Physics2D.Raycast(transform1.position, transform1.right, 0.8f,
                1 << LayerMask.NameToLayer("ground"));
        }

        private bool CheckGrass()
        {
            return Physics2D.OverlapCircle(groundCheck.position, GroundedRadius, 1 << LayerMask.NameToLayer("grass"));
        }

        private void Move(float move)
        {
            var velocity1 = body.velocity;
            body.velocity = Vector2.SmoothDamp(velocity1, new Vector2(move * 10f, velocity1.y), ref velocity,
                MovementSmoothing);

            //PlayerRun(!isOnCar ? Mathf.Abs(move) : 0f);
            if (isOnCar || CheckHitWall())
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

        private void Jumps()
        {
            if (mGrounded)
            {
                Jump();
                mGrounded = false;
                mDBJump = true;
                isDashing = true;
                playerAudio.Plays_13("Player_Jump");
            }
            else if (mDBJump)
            {
                Jump();
                mDBJump = false;
                isDashing = true;
                playerAudio.Plays_13("Player_Jump");
            }

        }

        private void Jump()
        {
            PlayerJump();
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
            // if (body.velocity.y > .1f)
            // {
            //     animator.SetBool("Is_Jump",true);
            // }else if (body.velocity.y < -.1f)
            // {
            //     animator.SetBool("Is_Falling",true);
            // }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Car"))
            {
                isOnCar = true;
            }
        }

        public void PlayerHurt()
        {
            playerAudio.Plays_10("Player_Hurt");
            animator.SetTrigger(animationState.playerIsHurt);
            body.bodyType = RigidbodyType2D.Static;
            isHurt = true;
            StartCoroutine(nameof(Hurting), 0.5f);
        }

        private IEnumerator Hurting(float delay)
        {
            yield return new WaitForSeconds(delay);
            body.bodyType = RigidbodyType2D.Dynamic;
            isHurt = false;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Car"))
            {
                isOnCar = false;
            }
        }
    }

    // public enum JumpState
    // {
    //     Normal,
    //     DoubleJump,
    //     None
    // }
}