using Script.Core;
using UnityEngine;

namespace Script.GamePlay
{
    public class ParallaxBackground : MonoBehaviour
    {
        private Camera _cam;
        [SerializeField] private Vector3 movementScale = Vector3.zero;

        private void Start()
        {
            _cam = Camera.main;
        }

        private void LateUpdate()
        {
            transform.position = Vector3.Scale(_cam.transform.position, movementScale);
        }
    }
}