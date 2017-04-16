using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class AutoMoveAndRotate : MonoBehaviour
    {
        public Vector3andSpace moveUnitsPerSecond;
        public Vector3andSpace rotateDegreesPerSecond;
        public bool ignoreTimescale;
        private float m_LastRealTime;


        private void Start()
        {
            this.m_LastRealTime = Time.realtimeSinceStartup;
        }


        // Update is called once per frame
        private void Update()
        {
            float deltaTime = Time.deltaTime;
            if (this.ignoreTimescale)
            {
                deltaTime = (Time.realtimeSinceStartup - this.m_LastRealTime);
                this.m_LastRealTime = Time.realtimeSinceStartup;
            }
            this.transform.Translate(this.moveUnitsPerSecond.value*deltaTime, this.moveUnitsPerSecond.space);
            this.transform.Rotate(this.rotateDegreesPerSecond.value*deltaTime, this.moveUnitsPerSecond.space);
        }


        [Serializable]
        public class Vector3andSpace
        {
            public Vector3 value;
            public Space space = Space.Self;
        }
    }
}
