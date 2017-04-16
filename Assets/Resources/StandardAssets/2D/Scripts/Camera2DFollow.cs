using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        // Use this for initialization
        private void Start()
        {
            this.m_LastTargetPosition = this.target.position;
            this.m_OffsetZ = (this.transform.position - this.target.position).z;
            this.transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (this.target.position - this.m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > this.lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                this.m_LookAheadPos = this.lookAheadFactor *Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                this.m_LookAheadPos = Vector3.MoveTowards(this.m_LookAheadPos, Vector3.zero, Time.deltaTime* this.lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = this.target.position + this.m_LookAheadPos + Vector3.forward* this.m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(this.transform.position, aheadTargetPos, ref this.m_CurrentVelocity, this.damping);

            this.transform.position = newPos;

            this.m_LastTargetPosition = this.target.position;
        }
    }
}
