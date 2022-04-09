using UnityEngine;
using TMPro;

namespace Game.GamePlay
{
    public class TargetFramerate : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtFPS;
        private const float PollingTime = 1f;
        private float time;
        private float frameCount;

        private void Start()
        {
            Application.targetFrameRate = -1;
        }

        private void LateUpdate()
        {
            time += Time.deltaTime;
            frameCount++;
            if (!(time >= PollingTime)) return;
            var frameRate = Mathf.RoundToInt(frameCount / time);
            txtFPS.text = frameRate + " FPS";
            time -= PollingTime;
            frameCount = 0f;
        }
    }
}