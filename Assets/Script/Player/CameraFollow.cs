using Script.Core;
using UnityEngine;

namespace Script.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float smoothValue;
        [SerializeField] private Vector2 offset;

        private void LateUpdate()
        {
            Debug.Log(HuyManager.Instance.IsLocalPlayer);
            if (!HuyManager.Instance.PlayerIsDeath())
            {
                Vector3 target = HuyManager.Instance.IsLocalPlayer.transform.position;
                Vector3 desiredPosition = new Vector3(target.x + offset.x, target.y + offset.y, -10f);
                transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothValue * Time.deltaTime);
            }
        }
    }
}