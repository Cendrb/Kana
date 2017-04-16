using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GameResources
{
    public class VehicleJointIdentifier
    {
        public int PartId { get; set; }
        public int JointId { get; set; }

        public VehicleJointIdentifier()
        {
            
        }

        public VehicleJointIdentifier(int partId, int jointId)
        {
            this.PartId = partId;
            this.JointId = jointId;
        }

        public override string ToString()
        {
            return string.Format("P{0}J{1}", this.PartId, this.JointId);
        }
    }
}
