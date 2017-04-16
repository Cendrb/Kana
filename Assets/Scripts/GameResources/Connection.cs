using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GameResources
{
    public class Connection
    { 
        public Connection(VehicleJointIdentifier joint1, VehicleJointIdentifier joint2)
        {
            if (joint1 == null)
            {
                throw new ArgumentNullException("joint1");
            }

            if (joint2 == null)
            {
                throw new ArgumentNullException("joint2");
            }

            this.Joint1Identifier = joint1;
            this.Joint2Identifier = joint2;
        }

        public VehicleJointIdentifier Joint1Identifier { get; set; }
        public VehicleJointIdentifier Joint2Identifier { get; set; }
    }
}
