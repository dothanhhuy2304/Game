using System.Collections;
using Game.Core;
using UnityEngine;
using Game.Player;

//Improver
namespace Game.Enemy
{
    public class WallMovement : BaseObject
    {
        public float speed = 3f;
        private Transform player;
        private Vector3 startPos = Vector3.zero;
        [SerializeField] private Vector2 endPos = Vector2.zero;
        [SerializeField] private float timeSleep;
        [SerializeField] private bool checkSleep;

        protected override void Start()
        {
            player = FindObjectOfType<CharacterController2D>()?.transform;
            startPos = transform.position;
        }

        protected override void Update()
        {
            if (CheckDistance(transform.position, player.position) < 30f || checkSleep)
            {
                transform.position = Vector3.Lerp(startPos, endPos, Mathf.PingPong(Time.time * speed, timeSleep));
            }
            else
            {
                StartCoroutine(nameof(ResetPos), 3f);
            }
        }

        private IEnumerator ResetPos(float reset)
        {
            yield return new WaitForSeconds(reset);
            transform.position = startPos;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = transform;
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player"))
            {
                player.transform.parent = null;
            }
        }
    }
}