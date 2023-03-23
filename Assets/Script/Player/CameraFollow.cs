using UnityEngine;

namespace Game.Player
{
    public class CameraFollow : MonoBehaviour
    {
        private CharacterController2D playerPosition;
        [SerializeField] private float smoothValue;
        [SerializeField] private Vector2 offset;

        private void Start()
        {
            playerPosition = CharacterController2D.instance;
        }

        private void LateUpdate()
        {
            if (!HuyManager.PlayerIsDeath())
            {
                Vector3 target = playerPosition.transform.position;
                Vector3 desiredPosition = new Vector3(target.x + offset.x, target.y + offset.y, -10f);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothValue * Time.deltaTime);
            }
        }
    }
}