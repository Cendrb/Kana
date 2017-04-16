using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof (Text))]
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        private Text m_Text;


        private void Start()
        {
            this.m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            this.m_Text = GetComponent<Text>();
        }


        private void Update()
        {
            // measure average frames per second
            this.m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > this.m_FpsNextPeriod)
            {
                this.m_CurrentFps = (int) (this.m_FpsAccumulator /fpsMeasurePeriod);
                this.m_FpsAccumulator = 0;
                this.m_FpsNextPeriod += fpsMeasurePeriod;
                this.m_Text.text = string.Format(display, this.m_CurrentFps);
            }
        }
    }
}
