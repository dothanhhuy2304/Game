using UnityEngine;

namespace Script.GamePlay
{
    public class TargetFramerate : FastSingleton<TargetFramerate>
    {
        //[SerializeField] private TextMeshProUGUI txtFPS;
        //private const float PollingTime = 1f;
        private float time;
        private float frameCount;

        private readonly GUIStyle styleButton = new GUIStyle();
        private Rect rectBtn;
        private int fpsCount;
        private float currentAvgFPS;
        private float msec;
        private float fps;
        private float deltaTime;

        protected override void Awake()
        {
            base.Awake();
            int w = Screen.width, h = Screen.height;
            SetRectButton(w, h);
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
            UpdateCumulativeMovingAverageFPS(fps);
            if (GUI.Button(rectBtn, $"{msec:00.0}ms {fps:000}fps {currentAvgFPS:000.0}Avg", styleButton))
            {
                ResetAvgFps();
            }
        }

        private void SetRectButton(int with, int height)
        {
            rectBtn.x = 0;
            rectBtn.y = 0;
            // ReSharper disable once PossibleLossOfFraction
            rectBtn.width = with / 2;
            // ReSharper disable once PossibleLossOfFraction
            rectBtn.height = height * 2 / 80;
            styleButton.alignment = TextAnchor.MiddleLeft;
            styleButton.fontSize = height * 2 / 80;
            styleButton.normal.textColor = Color.red;
        }
        
        private void UpdateCumulativeMovingAverageFPS(float newFPS)
        {
            ++fpsCount;
            currentAvgFPS += (newFPS - currentAvgFPS) / fpsCount;
            styleButton.normal.textColor = currentAvgFPS > 55.0f ? Color.green : Color.red;
        }

        private void ResetAvgFps()
        {
            currentAvgFPS = 0;
            fpsCount = 0;
        }

        // private void LateUpdate()
        // {
        //     time += Time.deltaTime;
        //     frameCount++;
        //     if (!(time >= PollingTime)) return;
        //     var frameRate = Mathf.RoundToInt(frameCount / time);
        //     txtFPS.text = frameRate + " FPS";
        //     time -= PollingTime;
        //     frameCount = 0f;
        // }
    }
}