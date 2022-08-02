using UnityEngine;

namespace Game.GamePlay
{
    public class ParallaxBackground : MonoBehaviour
    {
        private Camera camPos;
        [SerializeField] private Vector3 movementScale = Vector3.zero;

        private void Awake()
        {
            camPos = Camera.main;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Scale(camPos.transform.position, movementScale);
        }
    }
}