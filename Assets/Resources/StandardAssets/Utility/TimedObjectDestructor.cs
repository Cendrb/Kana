using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class TimedObjectDestructor : MonoBehaviour
    {
        [SerializeField] private float m_TimeOut = 1.0f;
        [SerializeField] private bool m_DetachChildren = false;


        private void Awake()
        {
            Invoke("DestroyNow", this.m_TimeOut);
        }


        private void DestroyNow()
        {
            if (this.m_DetachChildren)
            {
                this.transform.DetachChildren();
            }
            DestroyObject(this.gameObject);
        }
    }
}
