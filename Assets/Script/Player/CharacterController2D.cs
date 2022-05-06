using System.Collections;
using Game.Core;
using Game.GamePlay;
using UnityEngine;

namespace Game.Player
{
    public class CharacterController2D : BaseObject
    {
        [Header("Movement")] private const float MovementSmoothing = .05f;
        private Vector2 velocity = Vector2.zero;
        private float mHorizontal;

        [Space] [Header("Flip")] private bool mFacingRight = true;
        private bool isDashing;
        [SerializeField] private bool mGrounded;
        private const float GroundedRadius = .3f;
        [SerializeField] private Transform groundCheck;
        [Space] [SerializeField] private LayerMask whatIsGround;
        private bool isJump;
        private bool mDBJump;
        [SerializeField] private Animator animator;
        [SerializeField] private float clampMinX, clampMaxX;
        [SerializeField] private PlayerHealth playerHealth;
        private readonly AnimationStates animationState = new AnimationStates();
        private bool isOnCar;
        public bool isHurt;
        private float startSpeed;
        private int jumpCount;

        private PlayerAudio playerAudio;

        //private DeviceManager deviceManager;
        //private UnityEngine.EventSystems.EventTrigger btnLeft, btnRight, btnJump, btnAttack;
        //private Weapon weapon;

        protected override void Start()
        {
            base.Start();
            startSpeed = playerHealth.playerData.movingSpeed;
            playerAudio = FindObjectOfType<PlayerAudio>().GetComponent<PlayerAudio>();
            //deviceManager = FindObjectOfType<DeviceManager>().GetComponent<DeviceManager>();
            //weapon = GetComponent<Weapon>();
            //OnControl();
        }

        // private void OnControl()
        // {
        //     btnLeft = deviceManager.btnLeft.GetComponent<EventTrigger>();
        //     btnRight = deviceManager.btnRight.GetComponent<EventTrigger>();
        //     btnJump = deviceManager.btnJump.GetComponent<EventTrigger>();
        //     btnAttack = deviceManager.btnAttack.GetComponent<EventTrigger>();
        //     var eventBtnLeft = new EventTrigger.Entry();
        //     var eventBtnLeftUp = new EventTrigger.Entry();
        //     var eventBtnRight = new EventTrigger.Entry();
        //     var eventBtnRightUp = new EventTrigger.Entry();
        //     var eventBtnJump = new EventTrigger.Entry();
        //     var eventBtnAttack = new EventTrigger.Entry();
        //     eventBtnLeft.eventID = EventTriggerType.PointerDown;
        //     eventBtnLeftUp.eventID = EventTriggerType.PointerUp;
        //     eventBtnRight.eventID = EventTriggerType.PointerDown;
        //     eventBtnRightUp.eventID = EventTriggerType.PointerUp;
        //     eventBtnJump.eventID = EventTriggerType.PointerDown;
        //     eventBtnAttack.eventID = EventTriggerType.PointerDown;
        //     eventBtnLeft.callback.AddListener(t => { MobileMove(-1); });
        //     eventBtnLeftUp.callback.AddListener(t => { MobileMove(0); });
        //     eventBtnRight.callback.AddListener(t => { MobileMove(1); });
        //     eventBtnRightUp.callback.AddListener(t => { MobileMove(0); });
        //     eventBtnJump.callback.AddListener(t => { Jumps(); });
        //     eventBtnAttack.callback.AddListener(t => { weapon.Attacks(); });
        //     btnLeft.triggers.Add(eventBtnLeft);
        //     btnLeft.triggers.Add(eventBtnLeftUp);
        //     btnRight.triggers.Add(eventBtnRight);
        //     btnRight.triggers.Add(eventBtnRightUp);
        //     btnJump.triggers.Add(eventBtnJump);
        //     btnAttack.triggers.Add(eventBtnAttack);
        // }

        private void Update()
        {
            //if (SystemInfo.deviceType != DeviceType.Desktop) return;
            if (playerHealth.PlayerIsDeath() || body.bodyType == RigidbodyType2D.Static) return;
            OnCollision();
            if (isHurt) return;
            mHorizontal = Input.GetAxisRaw("Horizontal");
            if (!mGrounded || mDBJump == false)
            {
                if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1) && isDashing)
                {
                    Dash(mHorizontal);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                isJump = true;
                //Jumps();
            }
        }

        private void FixedUpdate()
        {
            if (playerHealth.PlayerIsDeath() || body.bodyType == RigidbodyType2D.Static) return;
            if (isHurt) return;
            Move(mHorizontal * playerHealth.playerData.movingSpeed * Time.fixedDeltaTime);

            if (isJump)
            {
                Jumps();
            }

            animator.SetFloat(animationState.playerJumpVelocity, body.velocity.y);

            if (body.velocity.y < -.1f && mGrounded)
            {
                jumpCount = 0;
                PlayerJump();
            }

            var position = transform.position;
            position = new Vector3(Mathf.Clamp(position.x, clampMinX, clampMaxX), position.y, position.z);
            body.transform.position = position;
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

        private void Move(float move)
        {
            var position = body.velocity;
            body.velocity = Vector2.SmoothDamp(position, new Vector2(move * 10f, position.y), ref velocity,
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

        private void MobileMove(float move)
        {
            mHorizontal = move;
        }

        private void Jumps()
        {
            // isJump = false;
            // if (mGrounded)
            // {
            //     jumpCount = 0;
            //     Jump();
            //     mDBJump = true;
            //     isDashing = true;
            //     playerAudio.Play("Player_Jump");
            //     //playerAudio.Plays_13("Player_Jump");
            //     jumpCount++;
            // }
            // else if (mDBJump)
            // {
            //     Jump();
            //     mDBJump = false;
            //     isDashing = true;
            //     playerAudio.Play("Player_Jump");
            //     //playerAudio.Plays_13("Player_Jump");
            //     jumpCount++;
            // }

            isJump = false;
            if (mGrounded)
            {
                jumpCount = 0;
                Jump();
                mDBJump = true;
                isDashing = true;
                jumpCount++;
            }
            else if (mDBJump)
            {
                Jump();
                mDBJump = false;
                isDashing = true;
                jumpCount++;
            }

            PlayerJump();
        }

        private void Jump()
        {
            //PlayerJump();
            body.velocity = new Vector2(body.velocity.x, 0f);
            body.AddForce(Vector2.up * playerHealth.playerData.jumpForce, ForceMode2D.Impulse);
            playerAudio.Play("Player_Jump");
        }

        private void Dash(float horizontal)
        {
            body.AddForce(Vector2.right * (horizontal * playerHealth.playerData.dashSpeed), ForceMode2D.Impulse);
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

        //Animator
        private void PlayerRun(float speed)
        {
            animator.SetFloat(animationState.playerRun, speed);
        }

        private void PlayerJump()
        {
            switch (jumpCount)
            {
                case 0:
                    animator.SetBool(animationState.playerIsJump, false);
                    animator.SetBool(animationState.playerIsDBJump, false);
                    break;
                case 1:
                    animator.SetBool(animationState.playerIsJump, true);
                    break;
                case 2:
                    animator.SetBool(animationState.playerIsJump, false);
                    animator.SetBool(animationState.playerIsDBJump, true);
                    break;
            }
        }

        public void PlayerDeath()
        {
            animator.SetTrigger(animationState.playerIsDeath);
            playerAudio.Play("Enemy_Death");
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Grass"))
            {
                playerHealth.playerData.movingSpeed -= 10;
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
                playerHealth.playerData.movingSpeed = startSpeed;
            }
        }

        public void PlayerHurt()
        {
            animator.SetTrigger(animationState.playerIsHurt);
            body.bodyType = RigidbodyType2D.Static;
            isHurt = true;
            playerAudio.Play("Player_Hurt");
            StartCoroutine(nameof(Hurting), 0.5f);
        }

        private IEnumerator Hurting(float delay)
        {
            yield return new WaitForSeconds(delay);
            body.bodyType = RigidbodyType2D.Dynamic;
            isHurt = false;
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