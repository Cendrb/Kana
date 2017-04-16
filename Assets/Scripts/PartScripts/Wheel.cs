using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.PartScripts
{
    class Wheel : Part
    {
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }
        public float MaxSteerSpeed { get; set; }
        public float BrakeSpeed { get; set; }


        private void FixedUpdate()
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                
                float angle = this.gameObject.transform.rotation.eulerAngles.z;
                Vector2 forceV = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                forceV = forceV * Acceleration;
                Vector2 pos = this.gameObject.transform.position + new Vector3(0.5f, 1);
                this.ParentVehicle.Rigidbody.AddForceAtPosition(forceV, pos);
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                float angle = this.gameObject.transform.rotation.eulerAngles.z;
                Vector2 forceV = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                forceV = forceV * Acceleration * -1;
                Vector2 pos = this.gameObject.transform.position + new Vector3(0.5f, 1);
                this.ParentVehicle.Rigidbody.AddForceAtPosition(forceV, pos);
            }
            else
            {

            }
        }
    }
}
