using UnityEngine;

namespace Game.Player
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform playerPosition;
        [SerializeField] private float smoothValue = 2f;
        [SerializeField] private float posX = 2f;
        [SerializeField] private float posY = 1f;
        private Vector3 targetPos = Vector3.zero;
        private PlayerHealth playerHealth;

        private void Start()
        {
            playerPosition = FindObjectOfType<CharacterController2D>().transform;
            Debug.Assert(playerPosition != null, nameof(playerPosition) + " != null");
            playerHealth = playerPosition.GetComponent<PlayerHealth>();
        }

        private void LateUpdate()
        {
            if (!playerPosition || playerHealth.PlayerIsDeath()) return;
            var position = playerPosition.position;
            targetPos = new Vector3(position.x + posX, position.y + posY, -10f);
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothValue * Time.deltaTime);
        }
    }
}