using UnityEngine;

namespace Script.GamePlay
{
    public class ParallaxBackground : MonoBehaviour
    {
        private Camera cam;
        [SerializeField] private Vector3 movementScale = Vector3.zero;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Scale(cam.transform.position, movementScale);
        }
    }
}