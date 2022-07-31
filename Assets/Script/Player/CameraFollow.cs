using UnityEngine;

namespace Game.Player
{
    public class CameraFollow : MonoBehaviour
    {
        private CharacterController2D playerPosition;
        [SerializeField] private float smoothValue = 2f;
        [SerializeField] private float posX = 2f;
        [SerializeField] private float posY = 1f;
        private Vector3 targetPos = Vector3.zero;

        private void Start()
        {
            playerPosition = CharacterController2D.instance;
        }

        private void LateUpdate()
        {
            if (!HuyManager.PlayerIsDeath())
            {
                var position = playerPosition.transform.position;
                targetPos = new Vector3(position.x + posX, position.y + posY, -10f);
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothValue * Time.deltaTime);
            }
        }
    }
}