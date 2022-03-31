using UnityEngine;

namespace Game.GamePlay
{
    public class ParallaxBackground : MonoBehaviour
    {
        private Transform camPos;
        [SerializeField] private Vector3 movementScale = Vector3.zero;

        private void Awake()
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            camPos = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Scale(camPos.position, movementScale);
        }
    }
}